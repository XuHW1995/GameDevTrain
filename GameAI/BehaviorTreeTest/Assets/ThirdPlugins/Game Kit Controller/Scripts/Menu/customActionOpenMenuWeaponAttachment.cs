using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customActionOpenMenuWeaponAttachment : customActionOpenMenuSystem
{
	public override void openOrCloseMenu (bool state, GameObject currentPlayer)
	{
		if (!customActionEnabled) {
			return;
		}

		playerComponentsManager currentPlayerComponentsManager = currentPlayer.GetComponent<playerComponentsManager> ();

		if (currentPlayerComponentsManager != null) {
			playerWeaponsManager currentPlayerWeaponsManager = currentPlayerComponentsManager.getPlayerWeaponsManager ();

			usingDevicesSystem currentUsingDevicesSystem = currentPlayerComponentsManager.getUsingDevicesSystem ();

			if (currentPlayerWeaponsManager != null) {
				if (currentPlayerWeaponsManager.isPlayerCarringWeapon ()) {
					if (state) {
						currentPlayerWeaponsManager.setOpenWeaponAttachmentsMenuPausedState (true);

						currentPlayerWeaponsManager.setIgnoreCheckUsingDevicesOnWeaponAttachmentsActiveState (true);

						currentUsingDevicesSystem.setIgnoreIfPlayerMenuActiveState (true);

						currentUsingDevicesSystem.setIgnoreIfUsingDeviceActiveState (true);
					}

					currentPlayerWeaponsManager.editWeaponAttachmentsByCheckingBusyState ();

					if (!state) {
						currentPlayerWeaponsManager.setOpenWeaponAttachmentsMenuPausedState (false);

						currentPlayerWeaponsManager.setIgnoreCheckUsingDevicesOnWeaponAttachmentsActiveState (false);

						currentUsingDevicesSystem.setIgnoreIfPlayerMenuActiveState (false);

						currentUsingDevicesSystem.setIgnoreIfUsingDeviceActiveState (false);
					}
				}
			}
		}
	}
}
