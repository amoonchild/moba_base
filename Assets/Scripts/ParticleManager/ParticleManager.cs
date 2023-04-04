using System.Collections.Generic;


namespace LiaoZhai.Runtime
{
    public static class ParticleManager
    {
        private static List<Particle> _particles = null;
        private static Dictionary<int, Particle> _particleDicts = null;
        private static Dictionary<string, Queue<Particle>> _destroyedParticles = null;


        public static void Init()
        {
            _particles = new List<Particle>();
            _particleDicts = new Dictionary<int, Particle>();
            _destroyedParticles = new Dictionary<string, Queue<Particle>>();
        }

        public static void Destroy()
        {
            for (int i = 0, count = _particles.Count; i < count; i++)
            {
                _particles[i].Release();
            }

            foreach (KeyValuePair<string, Queue<Particle>> item in _destroyedParticles)
            {
                while (item.Value.Count > 0)
                {
                    Particle particle = item.Value.Dequeue();
                    particle.Release();
                }
            }

            _particles.Clear();
            _particleDicts.Clear();
            _destroyedParticles.Clear();
        }

        public static void Release()
        {
            _particles = null;
            _particleDicts = null;
            _destroyedParticles = null;
        }

        public static void ChangeSpeed(float speed)
        {
            for (int i = 0, count = _particles.Count; i < count; i++)
            {
                _particles[i].ChangeSpeed(speed);
            }
        }

        public static void UpdateLogic(DFix64 frameLength)
        {
            for (int i = 0, count = _particles.Count; i < count;)
            {
                Particle particle = _particles[i];
                if (particle.CreatedFrame == BattleData.LogicFrame)
                {
                    break;
                }

                if (particle.IsDestroyed)
                {
                    particle.SetFree();

                    RecycleParticle(particle);
                    _particleDicts.Remove(particle.ObjectId);

                    count--;
                    _particles.RemoveAt(i);
                    continue;
                }
                else if (particle.IsFree)
                {
                    RecycleParticle(particle);
                    _particleDicts.Remove(particle.ObjectId);

                    count--;
                    _particles.RemoveAt(i);
                    continue;
                }

                particle.UpdateLogic(frameLength);
                i++;
            }
        }

        public static void UpdateRender(float interpolation, float deltaTime)
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                Particle particle = _particles[i];
                //if (particle.IsActivated && !particle.IsFree)
                {
                    particle.UpdateRender(interpolation, deltaTime);
                }
            }
        }

        public static int CreateParticle(string assetFullPath, bool isLoop)
        {
            Particle particle = InstantiateParticle(assetFullPath, isLoop);
            if (particle == null)
            {
                return 0;
            }

            _particles.Add(particle);
            _particleDicts.Add(particle.ObjectId, particle);

            particle.SetActive(true);
            particle.ChangeSpeed(BattleData.GetFinalBattleSpeed());

            return particle.ObjectId;
        }

        public static DropItemParticle CreateDropItemParticle(string assetFullPath)
        {
            DropItemParticle particle = InstantiateDropItemParticle(assetFullPath);
            if (particle == null)
            {
                return null;
            }

            _particles.Add(particle);
            _particleDicts.Add(particle.ObjectId, particle);

            particle.SetActive(true);

            return particle;
        }

        public static void SetParticleAttachTarget(int particleId, int attachTargetId)
        {
            if (_particleDicts.ContainsKey(particleId))
            {
                Particle particle = _particleDicts[particleId];
                if (!particle.IsFree && !particle.IsDestroyed)
                {
                    particle.AttachTargetId = attachTargetId;
                }
            }
        }

        public static void SetParticleAttach(int particleId, BaseUnit attachTarget, string attachPointName, bool isFollowPos, bool isFollowRot, bool isFollowScale, bool isAlwaysFollow)
        {
            if (_particleDicts.ContainsKey(particleId))
            {
                Particle particle = _particleDicts[particleId];
                particle.SetAttach(attachTarget, attachPointName, isFollowPos, isFollowRot, isFollowScale, isAlwaysFollow);
            }
        }

        public static void SetParticleControl(int particleId, ParticleControlType index, DFixVector3 control, bool isSync)
        {
            if (_particleDicts.ContainsKey(particleId))
            {
                Particle particle = _particleDicts[particleId];
                particle.SetParticleControl(index, control, isSync);
            }
        }

        public static void DestroyParticle(int particleId)
        {
            SetParticleFree(particleId);
        }

        public static void DestroyParticle(int particleId, int attachTargetId)
        {
            SetParticleFree(particleId, attachTargetId);
        }

        private static void SetParticleFree(int particleId)
        {
            if (_particleDicts.ContainsKey(particleId))
            {
                Particle particle = _particleDicts[particleId];
                if (particle.IsInstantiateComplete)
                {
                    particle.SetFree();
                }
                else
                {
                    particle.Destroy();
                }

                //RecycleParticle(particle);
                //_particleDicts.Remove(particle.ObjectId);
            }
        }

        private static void SetParticleFree(int particleId, int attachTargetId)
        {
            if (_particleDicts.ContainsKey(particleId))
            {
                Particle particle = _particleDicts[particleId];
                if (particle.AttachTargetId == attachTargetId)
                {
                    if (particle.IsInstantiateComplete)
                    {
                        particle.SetFree();
                    }
                    else
                    {
                        particle.Destroy();
                    }
                }

                //RecycleParticle(particle);
                //_particleDicts.Remove(particle.ObjectId);
            }
        }

        private static void RecycleParticle(Particle particle)
        {
            if (!string.IsNullOrEmpty(particle.AssetName))
            {
                if (_destroyedParticles.ContainsKey(particle.AssetName))
                {
                    _destroyedParticles[particle.AssetName].Enqueue(particle);
                }
                else
                {
                    Queue<Particle> particles = new Queue<Particle>();
                    particles.Enqueue(particle);
                    _destroyedParticles.Add(particle.AssetName, particles);
                }
            }
        }

        private static Particle InstantiateParticle(string assetFullPath, bool isLoop)
        {
            if (string.IsNullOrEmpty(assetFullPath))
            {
                return null;
            }

            Particle particle = null;
            if (_destroyedParticles.Count > 0)
            {
                if (_destroyedParticles.ContainsKey(assetFullPath))
                {
                    Queue<Particle> particles = _destroyedParticles[assetFullPath];
                    if (particles.Count > 0)
                    {
                        particle = particles.Dequeue();
                        particle.Reset();
                        particle.GetFree();
                    }
                }
            }

            if (particle == null)
            {
                particle = new Particle();
                particle.Init();
                particle.SetAsset(assetFullPath);
            }

            particle.SetIsLoop(isLoop);

            return particle;
        }

        private static DropItemParticle InstantiateDropItemParticle(string assetFullPath)
        {
            if (string.IsNullOrEmpty(assetFullPath))
            {
                return null;
            }

            DropItemParticle particle = null;
            if (_destroyedParticles.Count > 0)
            {
                if (_destroyedParticles.ContainsKey(assetFullPath))
                {
                    Queue<Particle> particles = _destroyedParticles[assetFullPath];
                    if (particles.Count > 0)
                    {
                        particle = (DropItemParticle)particles.Dequeue();
                        particle.Reset();
                        particle.GetFree();
                    }
                }
            }

            if (particle == null)
            {
                particle = new DropItemParticle();
                particle.Init();
                particle.SetAsset(assetFullPath);
            }

            particle.SetIsLoop(true);

            return particle;
        }
    }
}