using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

namespace Controller.Interaction.Icon
{
    public class NewIconInteractionController : GenericIconInteractionController
    {

        protected override void OnLeftClickOnTargetEventAction()
        {
            var op = GetOperator();

            Debug.Log("New Icon OnLeftClickEvent - on operator with ID: "+ op.Id);

            //test -- first click, just display all options; second click on one of those options, create respective operator!
            op.Observer.CreateOperator(1);
           


            // TODO suggest options to user after click on new operator. 
            var prefabs = op.Observer.GetOperatorPrefabs();
            foreach (var prefab in prefabs)
            {
                //first validate if the respective prefab can be used with the current parent
                if (prefab.GetComponent<GenericOperator>().ValidateIfOperatorPossibleForParents(op))
                {
                    // valid operator -> suggest spawn in list! -> i.e., popup / circular layout with icons
                    // get icon like this: 
                    // GameObject Icon = GameObject.Instantiate(prefab.GetComponentInChildren<GenericIcon>().gameObject);
                    // Note: this not only includes the texture, but also interaction controllers etc - you need to get rid of those! only copy texture -> implement new function in generic icon -> getTexture etc..
                    // Overwrite with spawn new prefab action required on click on one of those suggested prefabs

                    // replace new operator with the newly spawned one. 
                }
            }
        }
    }
}
