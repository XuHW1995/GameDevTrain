﻿using YooAsset;

namespace UniFramework.Pooling
{
	public class CreatePoolOperation : GameAsyncOperation
	{
		private enum ESteps
		{
			None,
			Waiting,
			Done,
		}

		private readonly AssetHandle _handle;
		private ESteps _steps = ESteps.None;

		internal CreatePoolOperation(AssetHandle handle)
		{
			_handle = handle;
		}
		protected override void OnStart()
		{
			_steps = ESteps.Waiting;
		}
		protected override void OnUpdate()
		{
			if (_steps == ESteps.None || _steps == ESteps.Done)
				return;

			if (_steps == ESteps.Waiting)
			{
				if (_handle.IsValid == false)
				{
					_steps = ESteps.Done;
					Status = EOperationStatus.Failed;
					Error = $"{nameof(AssetHandle)} is invalid.";
					return;
				}

				if (_handle.IsDone == false)
					return;

				if (_handle.AssetObject == null)
				{
					_steps = ESteps.Done;
					Status = EOperationStatus.Failed;
					Error = $"{nameof(AssetHandle.AssetObject)} is null.";
					return;
				}

				_steps = ESteps.Done;
				Status = EOperationStatus.Succeed;
			}
		}
		protected override void OnAbort()
		{
		}

		/// <summary>
		/// 等待异步实例化结束
		/// </summary>
		public void WaitForAsyncComplete()
		{
			if (_handle != null)
			{
				if (_steps == ESteps.Done)
					return;
				_handle.WaitForAsyncComplete();
				OnUpdate();
			}
		}
	}
}