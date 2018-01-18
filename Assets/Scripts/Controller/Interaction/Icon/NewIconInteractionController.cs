using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Controller.Interaction.Icon
{
    public class NewIconInteractionController : GenericIconInteractionController
    {
        private string _clickedButtonName;
        
        private void Update()
        {
            //get the current active collider or UI element
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                //get its name
                _clickedButtonName = EventSystem.current.currentSelectedGameObject.name;

                switch (_clickedButtonName)
                {
                        //spawn the selected operator and always a NewOperator in the same row as well
                        case "Option1":                                       
                            GetOperator().Observer.CreateOperator(0);
                            GetOperator().Observer.CreateOperator(1);                                              
                            break;
                            
                        case "Option2":
                            GetOperator().Observer.CreateOperator(2);
                            GetOperator().Observer.CreateOperator(1);
                            break;
                            
                        case "Option3":
                            //TODO add another operator here
                            break;
                            
                        case "Delete":
                            //TODO delete operator and all following ones in the same connected row
                            break;
                }  
                //set the event system back so the button isn't active anymore
                EventSystem.current.SetSelectedGameObject(null);
            } 
        }   
               
        protected override void OnLeftClickOnTargetEventAction()
        {
            var op = GetOperator();

            Debug.Log("New Icon OnLeftClickEvent - on operator with ID: "+ op.Id);

            //test -- first click, just display all options; second click on one of those options, create respective operator!
//            op.Observer.CreateOperator(0);
           


            // TODO suggest options to user after click on new operator. 
            //buttons for further interaction are spawned by clicking on the first newOperator icon
            UiController.ButtonSwitch();
            
//            Debug.Log(_clickedButtonName);
            
            
            var prefabs = op.Observer.GetOperatorPrefabs();
//            Debug.Log(prefabs[0]);
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
