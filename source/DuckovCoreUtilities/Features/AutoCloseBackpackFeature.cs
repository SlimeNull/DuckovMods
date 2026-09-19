using Duckov.UI;
using Duckov.UI.Inventories;
using SlimeNull.DuckovCoreUtilities.Infrastructure;
using SlimeNull.DuckovCoreUtilities.Utilities;
using UnityEngine.InputSystem;

namespace SlimeNull.DuckovCoreUtilities.Features
{
    internal class AutoCloseBackpackFeature : FeatureBase
    {
        private CharacterMainControl? _attachedCharacter;

        public override string Name => "Auto close backpack";

        public bool WhenMove { get; set; } = true;
        public bool WhenHurt { get; set; } = true;

        public bool DoNotCloseWhenInventoryHasMultiplePages { get; set; } = true;

        protected override void OnEnable()
        {
            LevelManager.OnControllingCharacterChanged += OnControllingCharacterChanged;

            var levelManager = LevelManager.Instance;
            if (levelManager != null &&
                levelManager.MainCharacter != null)
            {
                AttachToCharacter(levelManager.MainCharacter);
            }
        }

        protected override void OnDisable()
        {
            DetachFromCharacter();
            LevelManager.OnControllingCharacterChanged -= OnControllingCharacterChanged;
        }

        public override void Tick()
        {
            if (WhenMove &&
                View.ActiveView != null &&
                ShouldCloseViewThisFrame())
            {
                CloseCurrentView();
            }
        }

        private void OnControllingCharacterChanged(CharacterMainControl control)
        {
            AttachToCharacter(control);
        }

        private void AttachToCharacter(CharacterMainControl control)
        {
            DetachFromCharacter();
            if (control is null)
            {
                return;
            }

            control.Health.OnHurtEvent.AddListener(OnHurt);
            _attachedCharacter = control;
        }

        private void DetachFromCharacter()
        {
            if (_attachedCharacter is not null)
            {
                _attachedCharacter.Health.OnHurtEvent.RemoveListener(OnHurt);
                _attachedCharacter = null;
            }
        }

        private void OnHurt(DamageInfo arg0)
        {
            if (WhenHurt &&
                IsEnemyAttack(arg0))
            {
                CloseCurrentView();
            }
        }

        private bool IsEnemyAttack(DamageInfo damageInfo)
        {
            var attacker = damageInfo.fromCharacter;
            if (attacker == null ||
                _attachedCharacter == null ||
                damageInfo.isFromBuffOrEffect)
            {
                return false;
            }

            return attacker != _attachedCharacter &&
                attacker.Team != _attachedCharacter.Team;
        }

        private bool ShouldCloseViewThisFrame()
        {
            var keyboard = Keyboard.current;
            if (keyboard is null)
            {
                return false;
            }

            if (DoNotCloseWhenInventoryHasMultiplePages)
            {
                if (keyboard.wKey.wasPressedThisFrame ||
                    keyboard.sKey.wasPressedThisFrame)
                {
                    if (View.ActiveView is LootView lootView &&
                        lootView.GetLootTargetInventoryDisplay() is InventoryDisplay inventoryDisplay &&
                        inventoryDisplay.MaxPage > 1)
                    {
                        return false;
                    }

                    return true;
                }
            }

            return
                keyboard.aKey.wasPressedThisFrame ||
                keyboard.dKey.wasPressedThisFrame;
        }

        private static void CloseCurrentView()
        {
            View.ActiveView?.Close();
        }
    }
}
