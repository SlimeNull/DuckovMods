using Duckov.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SlimeNull.DuckovCoreUtilities.Utilities
{
    internal static class UnsafeAccessHelper
    {
        private static FieldInfo _lootTargetInventoryDisplayOfLootView = typeof(LootView)
            .GetField("lootTargetInventoryDisplay", BindingFlags.NonPublic | BindingFlags.Instance);

        public static InventoryDisplay GetLootTargetInventoryDisplay(this LootView lootView)
            => (InventoryDisplay)_lootTargetInventoryDisplayOfLootView.GetValue(lootView);
    }
}
