using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Riot.Phone.Service
{
    /// <summary>
    /// the UiActionBag is intended for the action to be executed on the UI thread
    /// </summary>
    public class UiActionBag
    {
        /// <summary>
        /// get the singleton instance
        /// </summary>
        public static UiActionBag Instance { get { return s_instance; } }

        /// <summary>
        /// add an action to the bag
        /// </summary>
        public void AddAction(BaseActionService actionService, object actionData)
        {
            UiActions.Add(new ActionDef { ActionData = actionData, ActionService = actionService });
        }

        /// <summary>
        /// take and execute one action from the ConcurrentBag if any item exist
        /// </summary>
        public ActionDef TryExecuteAction()
        {
            ActionDef action;
            if (UiActions.TryTake(out action))
            {
                action.ActionService.Act(action.ActionData);
            }
            return action;
        }

        /// <summary>
        /// take and execute all actions from the ConcurrentBag if any item exist
        /// </summary>
        public bool TryExecuteActions()
        {
            int count = 0;
            while (UiActions.Count > 0)
            {
                ActionDef action;
                if (UiActions.TryTake(out action))
                {
                    action.ActionService.Act(action.ActionData);
                    count++;
                }
            }
            return count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public class ActionDef
        {
            public BaseActionService ActionService { get; set; }
            public object ActionData { get; set; }
        }

        private UiActionBag() { }
        private ConcurrentBag<ActionDef> UiActions { get; set; } = new ConcurrentBag<ActionDef>();
        private static UiActionBag s_instance = new UiActionBag();
    }
}
