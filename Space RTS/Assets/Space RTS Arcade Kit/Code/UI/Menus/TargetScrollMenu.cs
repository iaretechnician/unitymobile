using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScrollMenu : ScrollMenuController
{

    public void PopulateMenuOptions(GameObject target)
    {
        HeaderText.text = "Target: " + target.name;

        AddMenuOption("Info").AddListener(() =>
        {
            OpenInfoMenu(target);
        });

        if (target.tag == "Ship" && target.GetComponent<Ship>().faction == Ship.PlayerShip.faction)
        {
            AddMenuOption("Commands").AddListener(() =>
            {
                SubMenu = GameObject.Instantiate(UIElements.Instance.SimpleMenu, transform.parent);
                RectTransform rt = SubMenu.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(
                    Screen.width / 2 + GetComponent<RectTransform>().sizeDelta.x / 2,
                    Screen.height / 2
                    );

                SimpleMenuController commsMenu = SubMenu.GetComponent<SimpleMenuController>();
                commsMenu.IsSubMenu = true;

                commsMenu.HeaderText.text = "AI Command Console" + target.name;
                ShipAI shipAI = target.GetComponent<ShipAI>();

                commsMenu.AddMenuOption("Move To ...").AddListener(() =>
                {
                    shipAI.MoveTo(null);
                });

                commsMenu.AddMenuOption("Follow me").AddListener(() =>
                {
                    shipAI.FollowMe();
                });

                commsMenu.AddMenuOption("Follow ...").AddListener(() =>
                {
                    shipAI.Follow(null);
                });

                commsMenu.AddMenuOption("Idle").AddListener(() =>
                {
                    shipAI.Idle();
                });

                commsMenu.AddMenuOption("Attack Enemies").AddListener(() =>
                {
                    shipAI.AttackAll();
                });

                if (target == Ship.PlayerShip.gameObject)
                    commsMenu.AddMenuOption("Attack Target").AddListener(() =>
                    {
                        GameObject UItarget = HUDMarkers.Instance.GetCurrentSelectedTarget();
                        if (UItarget != null)
                            shipAI.Attack(UItarget);
                    });

            });

            // If target is playership and has turrets
            if (target == Ship.PlayerShip.gameObject && target.GetComponent<ShipEquipment>().Turrets.Length > 0)
                AddMenuOption("Turret Commands").AddListener(() =>
                {
                    SubMenu = GameObject.Instantiate(UIElements.Instance.SimpleMenu, transform.parent);
                    RectTransform rt = SubMenu.GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(
                        Screen.width / 2 + GetComponent<RectTransform>().sizeDelta.x / 2,
                        Screen.height / 2
                        );

                    SimpleMenuController commsMenu = SubMenu.GetComponent<SimpleMenuController>();
                    commsMenu.IsSubMenu = true;

                    commsMenu.HeaderText.text = "Turret Control" + target.name;
                    ShipEquipment shipWeps = target.GetComponent<ShipEquipment>();

                    commsMenu.AddMenuOption("None").AddListener(() =>
                    {
                        shipWeps.SetTurretCommand(TurretCommands.TurretOrder.None);
                    });

                    commsMenu.AddMenuOption("Attack Enemies").AddListener(() =>
                    {
                        shipWeps.SetTurretCommand(TurretCommands.TurretOrder.AttackEnemies);
                    });

                    commsMenu.AddMenuOption("Attack Target").AddListener(() =>
                    {
                        shipWeps.SetTurretCommand(TurretCommands.TurretOrder.AttackTarget);
                    });

                    commsMenu.AddMenuOption("Manual").AddListener(() =>
                    {
                        shipWeps.SetTurretCommand(TurretCommands.TurretOrder.Manual);
                    });
                });

            // If is player owned
            if (target.GetComponent<Ship>().faction == Ship.PlayerShip.faction)
                AddMenuOption("Change Ship").AddListener(() =>
                {
                    Ship otherShip = target.GetComponent<Ship>();

                    Camera.main.GetComponent<FlightCameraController>().SetTargetShip(otherShip);
                    Ship.PlayerShip.IsPlayerControlled = false;
                    otherShip.IsPlayerControlled = true;
                    EquipmentIconUI.Instance.SetIconsForShip(otherShip);

                    OnCloseClicked();
                });
        }

        if (target.tag == "Station" || target.tag == "Ship")
        {
            AddMenuOption("Comms").AddListener(() =>
            {
                SubMenu = GameObject.Instantiate(UIElements.Instance.SimpleMenu, transform.parent);
                RectTransform rt = SubMenu.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(
                    Screen.width / 2 + GetComponent<RectTransform>().sizeDelta.x / 2,
                    Screen.height / 2
                    );

                SimpleMenuController commsMenu = SubMenu.GetComponent<SimpleMenuController>();
                commsMenu.IsSubMenu = true;

                commsMenu.HeaderText.text = "Comms open with " + target.name;

            });
        }

        if (target.tag == "Waypoint")
            AddMenuOption("Autpilot: Move To").AddListener(() =>
            {
                Ship.PlayerShip.AIInput.MoveTo(target.transform);
            });

    }

    public static void OpenInfoMenu(GameObject target)
    {
        var infoMenu = CanvasController.Instance.OpenMenu(UIElements.Instance.ScrollText);
        ScrollTextController infoCard = infoMenu.GetComponent<ScrollTextController>();

        if (target.tag == "Ship")
        {
            Ship ship = target.GetComponent<Ship>();

            infoCard.HeaderText.text = target.name;
            // Faction text in color according to relation with station
            float relation = Ship.PlayerShip.faction.RelationWith(ship.faction);
            infoCard.AddMenuItem("Faction: " + ship.faction.name, false,
                relation == 0 ? Color.white : (relation < 0 ? Color.red : Color.green));

            infoCard.AddMenuItem("Armor: " + ship.Armor, false, Color.red);
            infoCard.AddMenuItem("Weapons installed onboard: ", true, Color.black);
            int i = 0;
            foreach (var weapon in ship.Equipment.Guns)
            {
                if (weapon.mountedWeapon != null)
                    infoCard.AddMenuItem(
                       "Hardpoint " + i + ": " + weapon.mountedWeapon.name,
                       Color.white,
                       IconManager.Instance.GetWeaponIcon((int)IconManager.EquipmentIcons.Gun), 2f
                   );
                else
                    infoCard.AddMenuItem("Hardpoint " + i + ": [no weapon]",
                        Color.grey,
                        IconManager.Instance.GetWeaponIcon((int)IconManager.EquipmentIcons.Gun), 2f
                        );
                i++;
            }
            foreach (var weapon in ship.Equipment.Turrets)
            {
                if (weapon.mountedWeapon != null)
                    infoCard.AddMenuItem(
                        "Turret " + i + ": " + weapon.mountedWeapon.name,
                        Color.white,
                        IconManager.Instance.GetWeaponIcon((int)IconManager.EquipmentIcons.Turret), 2f
                    );
                else
                    infoCard.AddMenuItem("Hardpoint " + i + ": [no weapon]",
                        Color.grey,
                        IconManager.Instance.GetWeaponIcon((int)IconManager.EquipmentIcons.Turret), 2f
                        );
                i++;
            }

            infoCard.AddMenuItem("Equipment installed onboard: ", true, Color.black);
            foreach (var item in ship.Equipment.MountedEquipment)
            {
                infoCard.AddMenuItem(item.name, Color.white, IconManager.Instance.GetEquipmentIcon(item.name), 1f, 80);
            }

        }
    }
}
