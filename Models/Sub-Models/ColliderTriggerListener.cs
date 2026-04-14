using Rocket.Unturned.Chat;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models.Sub_Models
{
    public class ColliderTriggerListener : MonoBehaviour
    {
        internal ContainerModel Container { get; set; }
        public void OnTriggerEnter(Collider Other)
        {
            if (!Other.CompareTag("Vehicle"))
                return;

            InteractableVehicle Vehicle = Other.GetComponent<InteractableVehicle>();
            if (Vehicle == null)
                return;

            if (!Container.Vehicles.Contains(Vehicle))
            {
                Container.Vehicles.Add(Vehicle);
                if (Container.ActiveUI != null && Container.ActiveUI.IsActive && Container.ActiveUI is ManagingUiEffect)
                    (Container.ActiveUI as ManagingUiEffect).UpdateButtons();
            }
        }
        public void OnTriggerExit(Collider Other)
        {
            if (!Other.CompareTag("Vehicle"))
                return;

            InteractableVehicle Vehicle = Other.GetComponent<InteractableVehicle>();
            if (Vehicle == null)
                return;

            if (Container.Vehicles.Contains(Vehicle))
            {
                Container.Vehicles.Remove(Vehicle);
                if (Container.ActiveUI != null && Container.ActiveUI.IsActive && Container.ActiveUI is ManagingUiEffect)
                    (Container.ActiveUI as ManagingUiEffect).UpdateButtons();
                if (Container.ActiveDismantle != null && Container.ActiveDismantle.IsActive && Container.ActiveDismantle.Vehicle == Vehicle)
                    Container.ActiveDismantle.EndDismantle();
            }
        }
    }
}
