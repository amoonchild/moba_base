using System;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public abstract class BaseEntity : BaseObject
    {
        protected bool _isFree = false;
        protected string _assetName = string.Empty;                             // 资源路径
        protected int _entitySerialId = 0;                                      // 实体序列号
        protected bool _isReleased = false;                                     // 是否已销毁
        protected bool _isActivated = false;                                    // 是否显示
        protected bool _isFirstActivated = true;
        protected bool _isInstance = false;                                    // 是否已经销毁实体
        protected bool _isDestroyed = false;                                    // 是否已经销毁实体
        protected BattleEntityData _battleEntityData = null;                    // 实体数据
        protected bool _isInstantiateComplete = false;                          // 实体是否加载完成
        protected BattleEntityLogic _battleEntityLogic = null;                  // 实体对象
        protected DFixVector3 _lastLogicPosition = DFixVector3.Zero;              // 上一帧坐标
        protected DFixVector3 _lastLogicAngles = DFixVector3.Zero;                // 上一帧旋转
        protected DFixVector3 _lastLogicScale = DFixVector3.One;                  // 上一帧缩放
        protected DFixVector3 _logicPosition = DFixVector3.Zero;                  // 当前帧坐标
        protected DFixVector3 _logicEulerAngles = DFixVector3.Zero;               // 当前帧旋转
        protected DFixVector3 _logicScale = DFixVector3.One;                      // 当前帧缩放
        protected Vector3 _renderPosition = Vector3.zero;                       // 显示坐标
        protected Vector3 _renderEulerAngles = Vector3.zero;                    // 显示旋转
        protected Vector3 _renderScale = Vector3.one;                           // 显示缩放
        protected float _speed = 1f;

        public string AssetName
        {
            get
            {
                return _assetName;
            }
        }
        public bool IsReleased
        {
            get
            {
                return _isReleased;
            }
        }
        public bool IsActivated
        {
            get
            {
                return _isActivated;
            }
        }
        public bool IsDestroyed
        {
            get
            {
                return _isDestroyed;
            }
        }
        public bool IsInstantiateComplete
        {
            get { return _isInstantiateComplete; }
        }
        public bool IsFree
        {
            get { return _isFree; }
        }
        public virtual DFixVector3 LogicPosition
        {
            get
            {
                return _logicPosition;
            }
            set
            {
                _logicPosition = value;
            }
        }
        public virtual DFixVector3 LogicEulerAngles
        {
            get
            {
                return _logicEulerAngles;
            }
            set
            {
                _logicEulerAngles = value;
            }
        }
        public virtual DFixVector3 LogicScale
        {
            get
            {
                return _logicScale;
            }
            set
            {
                _logicScale = value;
            }
        }
        public Vector3 RenderPosition
        {
            get
            {
                return _renderPosition;
            }
        }
        public Vector3 RenderEulerAngles
        {
            get
            {
                return _renderEulerAngles;
            }
        }
        public Vector3 RenderScale
        {
            get
            {
                return _renderScale;
            }
        }


        public override void Init()
        {
            //_speed = BattleData.GetFinalBattleSpeed();
        }

        public override void Release()
        {
            if (_isReleased)
            {
                return;
            }

            _isReleased = true;
            _isInstance = false;

            if (_entitySerialId != 0)
            {
                Destroy();
                GameManager.GlobalData.FreeEntityId(_entitySerialId);
                _entitySerialId = 0;
            }

            _assetName = string.Empty;
            _battleEntityData = null;
            _battleEntityLogic = null;
        }

        public override void UpdateLogic(DFix64 frameLength)
        {
            if (IsActivated && !IsDestroyed && !IsFree)
            {
                _lastLogicPosition = _logicPosition;
                _lastLogicAngles = _logicEulerAngles;
                _lastLogicScale = _logicScale;
            }
        }

        public override void UpdateRender(float interpolation, float deltaTime)
        {
            if (IsActivated && !IsDestroyed && !IsFree)
            {
                _renderPosition = Vector3.Lerp(_lastLogicPosition.ToVector3(), _logicPosition.ToVector3(), interpolation);
                _renderEulerAngles = Vector3.Lerp(_lastLogicAngles.ToVector3(), _logicEulerAngles.ToVector3(), interpolation);
                _renderScale = Vector3.Lerp(_lastLogicScale.ToVector3(), _logicScale.ToVector3(), interpolation);

                if (_battleEntityLogic != null)
                {
                    _battleEntityLogic.CachedTransform.position = _renderPosition;
                    _battleEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
                    _battleEntityLogic.CachedTransform.localScale = _renderScale;
                }
            }
        }

        public virtual void SetAsset(string assetName)
        {
            if (_assetName != assetName)
            {
                if (_entitySerialId != 0)
                {
                    Destroy();
                    GameManager.GlobalData.FreeEntityId(_entitySerialId);
                    _entitySerialId = 0;
                }

                _assetName = assetName;
                if (!string.IsNullOrEmpty(_assetName) && _assetName != "empty")
                {
                    _entitySerialId = GameManager.GlobalData.GetGenerateEntityId();
                }
            }
        }

        public virtual void SetActive(bool isActivated)
        {
            if (_isActivated == isActivated && !_isFirstActivated)
            {
                return;
            }

            _isActivated = isActivated;
            _isFirstActivated = false;

            if (_isInstantiateComplete)
            {
                if (_isActivated)
                {
                    if (_battleEntityLogic != null && _battleEntityLogic.gameObject.layer != Constant.Battle.LAYER_DEFAULT)
                    {
                        _battleEntityLogic.gameObject.SetLayerRecursively(Constant.Battle.LAYER_DEFAULT);
                    }

                    SyncRender();
                    OnSetActive(true);
                }
                else
                {
                    if (_battleEntityLogic != null && _battleEntityLogic.gameObject.layer != Constant.Battle.LAYER_CULL)
                    {
                        _battleEntityLogic.gameObject.SetLayerRecursively(Constant.Battle.LAYER_CULL);
                    }

                    OnSetActive(false);
                }
            }
            else
            {
                if (_isActivated)
                {
                    SyncRender();
                    Instantiate();
                }
                else
                {
                    OnSetActive(false);
                }
            }
        }

        public virtual void SetFree()
        {
            if (_isFree)
            {
                return;
            }

            _isFree = true;
            _isActivated = false;

            OnSetFree();

            if (_isInstantiateComplete)
            {
                if (_battleEntityLogic != null)
                {
                    _battleEntityLogic.Recycle();
                    _battleEntityLogic.gameObject.SetActive(false);
                }
            }
            //else if (_entitySerialId != 0 && (GameManager.Entity.HasEntity(_entitySerialId) || GameManager.Entity.IsLoadingEntity(_entitySerialId)))
            //{
            //    GameManager.Entity.HideEntity(_entitySerialId);
            //}
        }

        public virtual void GetFree()
        {
            _isFree = false;
        }

        public virtual void Destroy()
        {
            if (_isDestroyed)
            {
                return;
            }

            _isInstance = false;
            _isDestroyed = true;
            _isInstantiateComplete = false;
            if (_battleEntityLogic != null)
            {
                _battleEntityLogic.Recycle();
                _battleEntityLogic = null;
            }
            _isFirstActivated = true;

            if (_entitySerialId == 0)
            {
                return;
            }

            if (GameManager.Entity.HasEntity(_entitySerialId) || GameManager.Entity.IsLoadingEntity(_entitySerialId))
            {
                GameManager.Entity.HideEntity(_entitySerialId);
            }
        }

        public virtual void ChangeSpeed(float speed)
        {
            _speed = speed;
        }

        public virtual void SyncRender()
        {
            _lastLogicPosition = _logicPosition;
            _lastLogicAngles = _logicEulerAngles;
            _lastLogicScale = _logicScale;

            _renderPosition = _logicPosition.ToVector3();
            _renderEulerAngles = _logicEulerAngles.ToVector3();
            _renderScale = _logicScale.ToVector3();

            if (_battleEntityLogic != null)
            {
                _battleEntityLogic.CachedTransform.position = _renderPosition;
                _battleEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
                _battleEntityLogic.CachedTransform.localScale = _renderScale;
            }
        }

        public virtual void SyncRenderPosition()
        {
            _lastLogicPosition = _logicPosition;

            _renderPosition = _logicPosition.ToVector3();
          
            if (_battleEntityLogic != null)
            {
                _battleEntityLogic.CachedTransform.position = _renderPosition;
            }
        }

        public virtual void SyncRenderRotation()
        {
            _lastLogicAngles = _logicEulerAngles;

            _renderEulerAngles = _logicEulerAngles.ToVector3();
           
            if (_battleEntityLogic != null)
            {
                _battleEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
            }
        }

        public virtual void SyncRenderScale()
        {
            _lastLogicScale = _logicScale;

            _renderScale = _logicScale.ToVector3();
           
            if (_battleEntityLogic != null)
            {
                _battleEntityLogic.CachedTransform.localScale = _renderScale;
            }
        }

        public virtual void LookToPositionY(DFixVector3 lookToPosition)
        {
            if (_logicPosition == lookToPosition)
            {
                return;
            }

            if (DFixVector3.Distance(_logicPosition, lookToPosition) == DFix64.Zero)
            {
                return;
            }

            try
            {
                Vector3 curr = _logicPosition.ToVector3();
                Vector3 lookTo = lookToPosition.ToVector3();
                Vector3 lookDir = lookTo - curr;
                lookDir = lookDir - Vector3.Project(lookDir, Vector3.up);
                //lookDir.Normalize();

                float angle = Vector3.Angle(Vector3.forward, lookDir);
                float dir = (Vector3.Dot(Vector3.up, Vector3.Cross(Vector3.forward, lookDir)) < 0 ? -1 : 1);
                angle *= dir;

                DFix64 newAngle = (DFix64)angle;
                if (newAngle < DFix64.Zero && _lastLogicAngles.y > DFix64.Zero)
                {
                    if (_lastLogicAngles.y - DFix64.Angel180 > newAngle)
                    {
                        _lastLogicAngles.y -= DFix64.Angel360;
                    }
                }
                else if (newAngle > DFix64.Zero && _lastLogicAngles.y < DFix64.Zero)
                {
                    if (_lastLogicAngles.y + DFix64.Angel180 < newAngle)
                    {
                        _lastLogicAngles.y += DFix64.Angel360;
                    }
                }

                _logicEulerAngles.y = newAngle;
            }
            catch (System.Exception e)
            {
                Log.Error(e.Message + "\n" + e.StackTrace);
            }
        }

        public virtual void OnShowEntitySuccess(BattleEntityLogic battleEntityLogic)
        {
            _isInstantiateComplete = true;
            _battleEntityLogic = battleEntityLogic;

            OnInstantiateSuccess();
            ChangeSpeed(_speed);

            if (IsActivated)
            {
                if(_battleEntityLogic.gameObject.layer != Constant.Battle.LAYER_DEFAULT)
                {
                    _battleEntityLogic.gameObject.SetLayerRecursively(Constant.Battle.LAYER_DEFAULT);
                }

                SyncRender();
                OnSetActive(IsActivated);
            }
            else
            {
                if (_battleEntityLogic.gameObject.layer != Constant.Battle.LAYER_CULL)
                {
                    _battleEntityLogic.gameObject.SetLayerRecursively(Constant.Battle.LAYER_CULL);
                }

                OnSetActive(IsActivated);
            }
        }

        public virtual void OnShowEntityFailure()
        {
            _isInstantiateComplete = true;
            _battleEntityLogic = null;

            OnInstantiateFailure();
        }

        protected abstract Type GetEntityType();

        protected virtual void Instantiate()
        {
            _isDestroyed = false;

#if UNITY_EDITOR
            if (BattleData.TestQuickBattle && BattleData.State == BattleState.Start)
            {
                _isInstantiateComplete = true;
                OnSetActive(IsActivated);
                return;
            }
#endif

            if (_entitySerialId != 0)
            {
                if (_battleEntityData == null)
                {
                    _battleEntityData = new BattleEntityData(this);
                }

                if (!GameManager.Entity.HasEntity(_entitySerialId) && !GameManager.Entity.IsLoadingEntity(_entitySerialId))
                {
                    GameManager.Entity.ShowEntity(_entitySerialId, GetEntityType(), _assetName, Constant.Entity.BATTLE_GROUP, _battleEntityData);
                }
            }
            else
            {
                _isInstantiateComplete = true;
                OnSetActive(IsActivated);
            }
        }

        protected virtual void OnInstantiateSuccess()
        {

        }

        protected virtual void OnInstantiateFailure()
        {

        }

        protected virtual void OnSetActive(bool isActivated)
        {
            if (_battleEntityLogic != null)
            {
                if (isActivated)
                {
                    _battleEntityLogic.CachedTransform.position = _renderPosition;
                    _battleEntityLogic.CachedTransform.eulerAngles = _renderEulerAngles;
                    _battleEntityLogic.CachedTransform.localScale = _renderScale;
                }

                _battleEntityLogic.gameObject.SetActive(isActivated);
            }
        }

        protected virtual void OnSetFree()
        {

        }
    }
}