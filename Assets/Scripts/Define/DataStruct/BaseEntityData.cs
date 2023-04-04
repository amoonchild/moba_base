using GameFramework.Resource;
using GameFramework.Event;
using GameFramework.Entity;
using UnityGameFramework.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace LiaoZhai.Runtime
{
    public class BattleEntityData
    {
        public BaseEntity Entity = null;


        public BattleEntityData(BaseEntity entity)
        {
            Entity = entity;
        }
    }
}