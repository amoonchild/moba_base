using System;
using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public static class ProjectileManager
    {
        private static List<BaseProjectile> _projectiles = null;
        private static Dictionary<int, BaseProjectile> _projectileDicts = null;
        private static Dictionary<string, Queue<BaseProjectile>> _destroyedAttackProjectiles = null;
        private static Dictionary<string, Queue<BaseProjectile>> _destroyedAbilityTrackProjectiles = null;
        private static Dictionary<string, Queue<BaseProjectile>> _destroyedAbilityLinerProjectiles = null;
        private static Type _typeAttackProjectile = null;
        private static Type _typeAbilityTrackingProjectile = null;
        private static Type _typeAbilityLinearProjectile = null;


        public static void Init()
        {
            _projectiles = new List<BaseProjectile>();
            _projectileDicts = new Dictionary<int, BaseProjectile>();
            _destroyedAttackProjectiles = new Dictionary<string, Queue<BaseProjectile>>();
            _destroyedAbilityTrackProjectiles = new Dictionary<string, Queue<BaseProjectile>>();
            _destroyedAbilityLinerProjectiles = new Dictionary<string, Queue<BaseProjectile>>();

            _typeAttackProjectile = typeof(AttackProjectile);
            _typeAbilityTrackingProjectile = typeof(AbilityTrackingProjectile);
            _typeAbilityLinearProjectile = typeof(AbilityLinearProjectile);
        }

        public static void Destroy()
        {
            for (int i = 0; i < _projectiles.Count; i++)
            {
                _projectiles[i].Release();
            }

            foreach (KeyValuePair<string, Queue<BaseProjectile>> item in _destroyedAttackProjectiles)
            {
                while (item.Value.Count > 0)
                {
                    BaseProjectile baseProjectile = item.Value.Dequeue();
                    baseProjectile.Release();
                }
            }

            foreach (KeyValuePair<string, Queue<BaseProjectile>> item in _destroyedAbilityTrackProjectiles)
            {
                while (item.Value.Count > 0)
                {
                    BaseProjectile baseProjectile = item.Value.Dequeue();
                    baseProjectile.Release();
                }
            }

            foreach (KeyValuePair<string, Queue<BaseProjectile>> item in _destroyedAbilityLinerProjectiles)
            {
                while (item.Value.Count > 0)
                {
                    BaseProjectile baseProjectile = item.Value.Dequeue();
                    baseProjectile.Release();
                }
            }

            _projectiles.Clear();
            _projectileDicts.Clear();
            _destroyedAttackProjectiles.Clear();
            _destroyedAbilityTrackProjectiles.Clear();
            _destroyedAbilityLinerProjectiles.Clear();
        }

        public static void Release()
        {
            _projectiles = null;
            _projectileDicts = null;
            _destroyedAttackProjectiles = null;
            _destroyedAbilityTrackProjectiles = null;
            _destroyedAbilityLinerProjectiles = null;

            _typeAttackProjectile = null;
            _typeAbilityTrackingProjectile = null;
            _typeAbilityLinearProjectile = null;
        }

        public static void ChangeSpeed(float speed)
        {
            for (int i = 0, count = _projectiles.Count; i < count; i++)
            {
                _projectiles[i].ChangeSpeed(speed);
            }
        }

        public static void UpdateLogic(DFix64 frameLength)
        {
            for (int i = 0, count = _projectiles.Count; i < count;)
            {
                BaseProjectile baseProjectile = _projectiles[i];
                if (baseProjectile.CreatedFrame == BattleData.LogicFrame)
                {
                    break;
                }

                if (baseProjectile.IsDestroyed)
                {
                    baseProjectile.SetFree();

                    if (_projectileDicts.ContainsKey(baseProjectile.ObjectId))
                    {
                        _projectileDicts.Remove(baseProjectile.ObjectId);
                    }

                    if (baseProjectile.GetType() == _typeAttackProjectile)
                    {
                        if (_destroyedAttackProjectiles.ContainsKey(baseProjectile.AssetName))
                        {
                            _destroyedAttackProjectiles[baseProjectile.AssetName].Enqueue(baseProjectile);
                        }
                        else
                        {
                            Queue<BaseProjectile> projectiles = new Queue<BaseProjectile>();
                            projectiles.Enqueue(baseProjectile);
                            _destroyedAttackProjectiles.Add(baseProjectile.AssetName, projectiles);
                        }
                    }
                    else if (baseProjectile.GetType() == _typeAbilityTrackingProjectile)
                    {
                        if (_destroyedAbilityTrackProjectiles.ContainsKey(baseProjectile.AssetName))
                        {
                            _destroyedAbilityTrackProjectiles[baseProjectile.AssetName].Enqueue(baseProjectile);
                        }
                        else
                        {
                            Queue<BaseProjectile> projectiles = new Queue<BaseProjectile>();
                            projectiles.Enqueue(baseProjectile);
                            _destroyedAbilityTrackProjectiles.Add(baseProjectile.AssetName, projectiles);
                        }
                    }
                    else if (baseProjectile.GetType() == _typeAbilityLinearProjectile)
                    {
                        if (_destroyedAbilityLinerProjectiles.ContainsKey(baseProjectile.AssetName))
                        {
                            _destroyedAbilityLinerProjectiles[baseProjectile.AssetName].Enqueue(baseProjectile);
                        }
                        else
                        {
                            Queue<BaseProjectile> projectiles = new Queue<BaseProjectile>();
                            projectiles.Enqueue(baseProjectile);
                            _destroyedAbilityLinerProjectiles.Add(baseProjectile.AssetName, projectiles);
                        }
                    }

                    count--;
                    _projectiles.RemoveAt(i);
                    continue;
                }
                else if (baseProjectile.IsFree)
                {
                    if (_projectileDicts.ContainsKey(baseProjectile.ObjectId))
                    {
                        _projectileDicts.Remove(baseProjectile.ObjectId);
                    }

                    if (baseProjectile.GetType() == _typeAttackProjectile)
                    {
                        if (_destroyedAttackProjectiles.ContainsKey(baseProjectile.AssetName))
                        {
                            _destroyedAttackProjectiles[baseProjectile.AssetName].Enqueue(baseProjectile);
                        }
                        else
                        {
                            Queue<BaseProjectile> projectiles = new Queue<BaseProjectile>();
                            projectiles.Enqueue(baseProjectile);
                            _destroyedAttackProjectiles.Add(baseProjectile.AssetName, projectiles);
                        }
                    }
                    else if (baseProjectile.GetType() == _typeAbilityTrackingProjectile)
                    {
                        if (_destroyedAbilityTrackProjectiles.ContainsKey(baseProjectile.AssetName))
                        {
                            _destroyedAbilityTrackProjectiles[baseProjectile.AssetName].Enqueue(baseProjectile);
                        }
                        else
                        {
                            Queue<BaseProjectile> projectiles = new Queue<BaseProjectile>();
                            projectiles.Enqueue(baseProjectile);
                            _destroyedAbilityTrackProjectiles.Add(baseProjectile.AssetName, projectiles);
                        }
                    }
                    else if (baseProjectile.GetType() == _typeAbilityLinearProjectile)
                    {
                        if (_destroyedAbilityLinerProjectiles.ContainsKey(baseProjectile.AssetName))
                        {
                            _destroyedAbilityLinerProjectiles[baseProjectile.AssetName].Enqueue(baseProjectile);
                        }
                        else
                        {
                            Queue<BaseProjectile> projectiles = new Queue<BaseProjectile>();
                            projectiles.Enqueue(baseProjectile);
                            _destroyedAbilityLinerProjectiles.Add(baseProjectile.AssetName, projectiles);
                        }
                    }

                    count--;
                    _projectiles.RemoveAt(i);
                    continue;
                }

                baseProjectile.UpdateLogic(frameLength);
                i++;
            }
        }

        public static void UpdateRender(float interpolation, float deltaTime)
        {
            for (int i = 0; i < _projectiles.Count; i++)
            {
                BaseProjectile baseProjectile = _projectiles[i];
                //if (baseProjectile.IsActivated && !baseProjectile.IsFree)
                {
                    baseProjectile.UpdateRender(interpolation, deltaTime);
                }
            }
        }

        public static int CreateAttackProjectile(string assetFullPath, CreateAttackProjectileData createData)
        {
            AttackProjectile projectile = InstantiateAttackProjectile(!string.IsNullOrEmpty(assetFullPath) ? assetFullPath : "empty");
            if (projectile == null)
            {
                return 0;
            }

            _projectiles.Add(projectile);
            _projectileDicts.Add(projectile.ObjectId, projectile);

            projectile.Launch(createData);
            projectile.ChangeSpeed(BattleData.GetFinalBattleSpeed());

            return projectile.ObjectId;
        }

        public static int CreateAbilityTrackProjectile(CreateAbilityTrackingProjectileData createData)
        {
            AbilityTrackingProjectile projectile = InstantiateAbilityTrackProjectile(!string.IsNullOrEmpty(createData.EffectName) ? createData.EffectName : "empty");
            if (projectile == null)
            {
                return 0;
            }

            _projectiles.Add(projectile);
            _projectileDicts.Add(projectile.ObjectId, projectile);

            projectile.Launch(createData);
            projectile.ChangeSpeed(BattleData.GetFinalBattleSpeed());

            return projectile.ObjectId;
        }

        public static int CreateAbilityLinerProjectile(CreateAbilityLinearProjectileData createData)
        {
            AbilityLinearProjectile projectile = InstantiateAbilityLinerProjectile(!string.IsNullOrEmpty(createData.EffectName) ? createData.EffectName : "empty");
            if (projectile == null)
            {
                return 0;
            }

            _projectiles.Add(projectile);
            _projectileDicts.Add(projectile.ObjectId, projectile);

            projectile.Launch(createData);
            projectile.ChangeSpeed(BattleData.GetFinalBattleSpeed());

            return projectile.ObjectId;
        }

        public static void DestroyProjectile(int id)
        {
            if (_projectileDicts.ContainsKey(id))
            {
                BaseProjectile baseProjectile = _projectileDicts[id];
                if (baseProjectile.IsInstantiateComplete)
                {
                    baseProjectile.SetFree();
                }
                else
                {
                    baseProjectile.Destroy();
                }

                //_projectileDicts.Remove(baseProjectile.ObjectId);

                //if (baseProjectile.GetType() == _typeAttackProjectile)
                //{
                //    if (_destroyedAttackProjectiles.ContainsKey(baseProjectile.AssetName))
                //    {
                //        _destroyedAttackProjectiles[baseProjectile.AssetName].Enqueue(baseProjectile);
                //    }
                //    else
                //    {
                //        Queue<BaseProjectile> projectiles = new Queue<BaseProjectile>();
                //        projectiles.Enqueue(baseProjectile);
                //        _destroyedAttackProjectiles.Add(baseProjectile.AssetName, projectiles);
                //    }
                //}
                //else if (baseProjectile.GetType() == _typeAbilityTrackingProjectile)
                //{
                //    if (_destroyedAbilityTrackProjectiles.ContainsKey(baseProjectile.AssetName))
                //    {
                //        _destroyedAbilityTrackProjectiles[baseProjectile.AssetName].Enqueue(baseProjectile);
                //    }
                //    else
                //    {
                //        Queue<BaseProjectile> projectiles = new Queue<BaseProjectile>();
                //        projectiles.Enqueue(baseProjectile);
                //        _destroyedAbilityTrackProjectiles.Add(baseProjectile.AssetName, projectiles);
                //    }
                //}
                //else if (baseProjectile.GetType() == _typeAbilityLinearProjectile)
                //{
                //    if (_destroyedAbilityLinerProjectiles.ContainsKey(baseProjectile.AssetName))
                //    {
                //        _destroyedAbilityLinerProjectiles[baseProjectile.AssetName].Enqueue(baseProjectile);
                //    }
                //    else
                //    {
                //        Queue<BaseProjectile> projectiles = new Queue<BaseProjectile>();
                //        projectiles.Enqueue(baseProjectile);
                //        _destroyedAbilityLinerProjectiles.Add(baseProjectile.AssetName, projectiles);
                //    }
                //}
            }
        }

        private static AttackProjectile InstantiateAttackProjectile(string assetFullPath)
        {
            AttackProjectile projectile = null;
            if (_destroyedAttackProjectiles.Count > 0)
            {
                if (_destroyedAttackProjectiles.ContainsKey(assetFullPath))
                {
                    Queue<BaseProjectile> projectiles = _destroyedAttackProjectiles[assetFullPath];
                    if (projectiles.Count > 0)
                    {
                        projectile = (AttackProjectile)projectiles.Dequeue();
                        projectile.Reset();
                        projectile.GetFree();
                    }
                }
            }

            if (projectile == null)
            {
                projectile = new AttackProjectile();
                projectile.SetAsset(assetFullPath);
            }

            return projectile;
        }

        private static AbilityTrackingProjectile InstantiateAbilityTrackProjectile(string assetFullPath)
        {
            AbilityTrackingProjectile projectile = null;
            if (_destroyedAbilityTrackProjectiles.Count > 0)
            {
                if (_destroyedAbilityTrackProjectiles.ContainsKey(assetFullPath))
                {
                    Queue<BaseProjectile> projectiles = _destroyedAbilityTrackProjectiles[assetFullPath];
                    if (projectiles.Count > 0)
                    {
                        projectile = (AbilityTrackingProjectile)projectiles.Dequeue();
                        projectile.Reset();
                        projectile.GetFree();
                    }
                }
            }

            if (projectile == null)
            {
                projectile = new AbilityTrackingProjectile();
                projectile.SetAsset(assetFullPath);
            }

            return projectile;
        }

        private static AbilityLinearProjectile InstantiateAbilityLinerProjectile(string assetFullPath)
        {
            AbilityLinearProjectile projectile = null;
            if (_destroyedAbilityLinerProjectiles.Count > 0)
            {
                if (_destroyedAbilityLinerProjectiles.ContainsKey(assetFullPath))
                {
                    Queue<BaseProjectile> projectiles = _destroyedAbilityLinerProjectiles[assetFullPath];
                    if (projectiles.Count > 0)
                    {
                        projectile = (AbilityLinearProjectile)projectiles.Dequeue();
                        projectile.Reset();
                        projectile.GetFree();
                    }
                }
            }

            if (projectile == null)
            {
                projectile = new AbilityLinearProjectile();
                projectile.SetAsset(assetFullPath);
            }

            return projectile;
        }
    }
}