using System.Collections.Generic;

namespace ShmupBoss
{
    /// <summary>
    /// A pivotal class for orgnizing the order of execution of different methods.<br></br>
    /// Certain objects need to be pooled before spawned for example, and these 2 actions are located 
    /// in 2 completely different classes, others need to be listed before pooled, etc...<br></br>
    /// This class contains a list of methods with an order index that allows for an orderly
    /// execution of things.<br></br> 
    /// If you would like to see the order of how things are processed, select the subscribe 
    /// to stage function and see where it is used (find all references) and observe the actions 
    /// and stage numbers.
    /// </summary>
    public class Initializer : Singleton<Initializer>
    {
        /// <summary>
        /// All the different methods which are subscibed to this class are indexed
        /// here after saving them with their stage order.
        /// </summary>
        private List<StagedMethod> stagedMethods = new List<StagedMethod>();

        private void Start()
        {
            stagedMethods.Sort();

            foreach (StagedMethod sm in stagedMethods)
            {
                sm.Execute();
            }

            stagedMethods.Clear();
            Destroy(gameObject);
        }

        /// <summary>
        /// Subscribes your action to the Initializer to orderly execute it according
        /// to the stage index.<br></br> 
        /// Please only subscribe your stage in the Unity Awake hook as these methods are sorted in start,
        /// subscribing at Start instead of Awake means it is very possible your method will not be executed.
        /// </summary>
        /// <param name="method">The method you wish to subscribe to the initializer and would like to execute 
        /// according to a stage index</param>
        /// <param name="stageOrder">The order in which a method is executed</param>
        /// <returns>True when a new staged method have been created and added and false
        /// when the method have been added to a pre-existing staged method.</returns>
        public bool SubscribeToStage(System.Action method, int stageOrder)
        {
            int indexOfStagedMethodInList = -1;

            for (int i = 0; i < stagedMethods.Count; i++)
            {
                if (stagedMethods[i].StageOrder == stageOrder)
                {
                    indexOfStagedMethodInList = i;
                    break;
                }
            }

            if (indexOfStagedMethodInList == -1)
            {
                stagedMethods.Add(new StagedMethod(method, stageOrder));
                return true;
            }
            else
            {
                stagedMethods[indexOfStagedMethodInList].SubscribeToOnStageCall(method);
                return false;
            }
        }
    }
}