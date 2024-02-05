using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for enemy and player weapons which emit particles as bullets.
    /// </summary>
    public abstract class WeaponParticle : Weapon
    {
        public event CoreDelegate OnStartDestroy;
        public event CoreDelegate OnParticleHit;

        protected float Damage;
        protected float Size;
        protected float ColliderSize;
        protected float Speed;

        /// <summary>
        /// How fast will the particle bullet rotate around itself.
        /// </summary>
        protected float RotationSpeed;

        /// <summary>
        /// Offsets the fired radial particles away from the weapon center by the radius value.
        /// </summary>
        protected float ArcRadius;

        /// <summary>
        /// The distance particles will travel before being eliminated, a distance of 0 means they will continue 
        /// traveling infinitely and you would need to use a particle destroyed to eliminate them instead.
        /// </summary>
        protected float Distance;

        /// <summary>
        /// After the agent with the weapon which uses this particle stage is despawned, the weapon needs to stay 
        /// active for a time. If the weapon is deactivated immediately; all of its particles will disappear  
        /// instantly, this time determines for how long will the particle weapon stay active after its agent 
        /// has been despawned.
        /// </summary>
        protected float Overtime;

        /// <summary>
        /// Would make the particle follow the rotation of the weapon which is firing them otherwise they 
        /// will be unaffected.
        /// </summary>
        protected bool IsFollowingRotation;

        /// <summary>
        /// The material fired particles will have. This practically holds the sprite which will be used 
        /// for the particles.
        /// </summary>
        protected UnityEngine.Object BulletMaterial;

        /// <summary>
        /// The sound effect made when a weapon fires a particle.
        /// </summary>
        protected UnityEngine.Object FiringSfx;

        /// <summary>
        /// If added, this material will be added as a trail for every fired particle.
        /// </summary>
        protected UnityEngine.Object TrailMaterial;

        /// <summary>
        /// Determines how long the particle trail will be.
        /// </summary>
        protected float TrailTime;

        protected float SfxVolume;

        protected float TrailWidth;

        /// <summary>
        /// The curve that will hold how the particles move over time, can be used to change particles 
        /// movement from a straight line into a curved line.
        /// </summary>
        protected AnimationCurve Curve;

        protected float CurveRange;

        public ParticleSystem PS;
        public ParticleSystemRenderer Psr;
        protected List<ParticleCollisionEvent> pces = new List<ParticleCollisionEvent>();

        public bool IsInitialized;

        /// <summary>
        /// The layer number of the opposing faction which particles damage.
        /// </summary>
        protected virtual int layerToHit
        {
            get;
        }

        public virtual WeaponParticleStage CurrentStage
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// This multiplier is affected by the level multiplier data.
        /// </summary>
        public abstract float DamageMultiplier
        {
            get;
        }

        /// <summary>
        /// This multiplier is affected by the level multiplier data.
        /// </summary>
        public abstract float SpeedMultiplier
        {
            get;
        }

        protected virtual void Start()
        {
            Initialize();
            SetToStage(CurrentStage);
        }

        protected virtual void OnParticleCollision(GameObject HitTarget)
        {
            PS.GetCollisionEvents(HitTarget, pces);

            ParticleCollisionEvent pce = pces[pces.Count - 1];

            RaiseOnParticleHit(new ParticleHitArgs(
                Math2D.VectorToDegree(pce.normal),
                pce.intersection, 
                HitTarget));
        }

        public virtual void Initialize()
        {            
            CacheParticleSystems();
            InitializeMainModule();
            InitializeCollisionModule();
            InitializeShapeModule();
            InitializeEmission();

            IsInitialized = true;
        }

        private void CacheParticleSystems()
        {
            if (PS == null)
            {
                PS = GetComponent<ParticleSystem>();
            }

            if (Psr == null)
            {
                Psr = GetComponent<ParticleSystemRenderer>();
            }            
        }

        private void InitializeMainModule()
        {
            ParticleSystem.MainModule psMain = PS.main;
            psMain.simulationSpace = ParticleSystemSimulationSpace.World;
        }

        private void InitializeCollisionModule()
        {
            ParticleSystem.CollisionModule psCollision = PS.collision;

            psCollision.enabled = true;
            psCollision.type = ParticleSystemCollisionType.World;
            psCollision.lifetimeLoss = 1;
            psCollision.mode = ParticleSystemCollisionMode.Collision2D;
            psCollision.sendCollisionMessages = true;
            psCollision.enableDynamicColliders = false;

            int layerToHitBitwise = 1 << layerToHit;
            int layerToBeDestroyedAtBitWise = 1 << ProjectLayers.ParticleDestroyer;
            int layersToCollideWith = layerToHitBitwise | layerToBeDestroyedAtBitWise;

            psCollision.collidesWith = layersToCollideWith;
        }

        protected void InitializeShapeModule()
        {
            ParticleSystem.ShapeModule psShape = PS.shape;

            psShape.enabled = true;

            // set shapeType to edge and set that edge length to 0.01f so 
            // the particles will start at the same point and have same direction.
            psShape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
            psShape.radius = 0.01f;
        }

        private void InitializeEmission()
        {
            ParticleSystem.EmissionModule psEmission = PS.emission;

            psEmission.enabled = false;
        }

        public virtual void SetToStage(WeaponParticleStage stage)
        {
            SetStageVariables(stage);
            setRotationBySpeedModule();
            SetColllisionModule();
            SetMaterial();
            SetVelocityAndCurveRange();
            SetTrailMaterial();
        }

        private void SetStageVariables(WeaponParticleStage stage)
        {
            rate = stage.Rate;
            Damage = stage.Damage * DamageMultiplier;
            Size = stage.Size;
            ColliderSize = stage.ColliderSize;
            Speed = stage.Speed * SpeedMultiplier;
            RotationSpeed = stage.RotationSpeed;

            BulletsNumber = stage.BulletsNumber;
            ArcAngleSpread = stage.ArcAngleSpread;
            ArcRadius = stage.ArcRadius;

            Distance = stage.Distance;
            Overtime = stage.Overtime;

            IsFollowingRotation = stage.IsFollowingRotation;

            BulletMaterial = stage.BulletMaterial;

            FiringSfx = stage.FiringSFX;
            SfxVolume = stage.SfxVolume;
            SfxCoolDownTime = stage.SfxCoolDownTime;

            TrailMaterial = stage.TrailMaterial;
            TrailTime = stage.TrailTime;
            TrailWidth = stage.TrailWidth;

            Curve = stage.Curve;
            CurveRange = stage.CurveRange;
        }

        private void setRotationBySpeedModule()
        {
            ParticleSystem.RotationBySpeedModule psRotation = PS.rotationBySpeed;

            if(RotationSpeed != 0.0f)
            {
                psRotation.enabled = true;
                psRotation.separateAxes = true;
                psRotation.zMultiplier = Mathf.Deg2Rad * RotationSpeed;
            }
            else
            {
                psRotation.enabled = false;
            }
        }

        private void SetColllisionModule()
        {
            ParticleSystem.CollisionModule psCollision = PS.collision;
            psCollision.radiusScale = ColliderSize;
        }

        private void SetMaterial()
        {
            if (BulletMaterial == null)
            {
                return;
            }

            if ((Material)BulletMaterial == null)
            {
                return;
            }

            Psr.material = (Material)BulletMaterial;
        }

        private void SetVelocityAndCurveRange()
        {
            ParticleSystem.VelocityOverLifetimeModule velocity = PS.velocityOverLifetime;
            velocity.enabled = true;

            velocity.x = new ParticleSystem.MinMaxCurve(CurveRange, Curve);
            ParticleSystem.MinMaxCurve defaultCurve = new ParticleSystem.MinMaxCurve(0, new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) }));
            velocity.y = defaultCurve;
            velocity.z = defaultCurve;
        }

        private void SetTrailMaterial()
        {
            ParticleSystem.TrailModule trailModule = PS.trails;

            if (TrailMaterial == null)
            {
                trailModule.enabled = false;
                return;
            }

            trailModule.enabled = true;
            Psr.trailMaterial = (Material)TrailMaterial;
            trailModule.lifetime = TrailTime;
            trailModule.widthOverTrail = new ParticleSystem.MinMaxCurve(
                TrailWidth,
                new AnimationCurve(new Keyframe[]
                {
                    new Keyframe(0, 1),
                    new Keyframe(1, 0)
                }));
        }

        public override void Fire(Vector2 agentDirection)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            float agentDirectionAngle = Math2D.VectorToDegree(agentDirection);

            if (BulletsNumber <= 1)
            {
                float emitAngle = agentDirectionAngle + (transform.localEulerAngles.z);

                PS.Emit(FindEmitParams(emitAngle), 1);
            }
            else
            {
                float arcStartAngle = agentDirectionAngle + (ArcAngleSpread * 0.5f) + (transform.localEulerAngles.z * 0.5f);
                float arcStepAngle = ArcAngleSpread / (BulletsNumber - 1);

                for (int i = 0; i < BulletsNumber; i++)
                {
                    float emitAngle = arcStartAngle - (i * arcStepAngle) + (transform.localEulerAngles.z * 0.5f);

                    PS.Emit(FindEmitParams(emitAngle), 1);
                }
            }
        }

        private ParticleSystem.EmitParams FindEmitParams(float emitAngle)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

            if (IsFollowingRotation)
            {
                emitParams.rotation = -emitAngle + 90.0f;
            }

            Vector3 dir = Math2D.DegreeToVector3(emitAngle);
            emitParams.velocity = dir * Speed;
            emitParams.position = transform.position + (dir * ArcRadius);

            emitParams.startSize = Size;

            if(Distance > 0.0f)
            {
                emitParams.startLifetime = Distance / Speed;
            }

            return emitParams;
        }

        protected override void StartFiringSoundCoroutine()
        {
            if (FiringSfx == null)
            {
                return;
            }

            if ((AudioClip)FiringSfx == null)
            {
                return;
            }

            if (SfxCoolDownTime <= 0.0f)
            {
                SfxSource.Instance.PlayClip((AudioClip)FiringSfx, SfxVolume);
            }
            else
            {
                StartCoroutine(PlayFiringSound());
            }
        }

        protected IEnumerator PlayFiringSound()
        {
            if (!isInFiringSfxCooldown)
            {
                SfxSource.Instance.PlayClip((AudioClip)FiringSfx, SfxVolume);
                isInFiringSfxCooldown = true;
                yield return new WaitForSeconds(SfxCoolDownTime);
                isInFiringSfxCooldown = false;
            }
        }

        public virtual void Decommision()
        {
            RaiseOnStartDestroy();

            Destroy(gameObject, Overtime);
        }

        protected void RaiseOnStartDestroy()
        {
            OnStartDestroy?.Invoke(null);
        }

        protected void RaiseOnParticleHit(ParticleHitArgs args)
        {
            OnParticleHit?.Invoke(args);
        }
    }
}