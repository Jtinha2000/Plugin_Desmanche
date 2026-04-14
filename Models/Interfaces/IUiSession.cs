using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dismantle_Plugin__UnturnedStore_Version_.Models.Interfaces
{
    public interface IUiSession
    {
        bool IsActive { get; }
        bool IsLoadingConstructor { get; set; }
        ContainerModel Data { get; set; }
        Player Owner { get; set; }
        void EndSession();
        void Instantiate();
        void EffectManager_OnButtonClicked(string ButtonName);
        void EffectManager_OnInputFieldSubmitted(string FieldName, string InputText);
    }
}
