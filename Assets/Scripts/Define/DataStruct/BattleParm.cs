using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;


namespace LiaoZhai.Runtime
{
    public class BattleParm
    {
        public DFix64 Parm15 = (DFix64)0.4f;
        public DFix64 Parm16 = (DFix64)200;
        public DFix64 Parm17 = (DFix64)5000;
        public DFix64 Parm18 = (DFix64)1;
        public DFix64 Parm19 = (DFix64)100;
        public DFix64 Parm20 = (DFix64)0;
        public DFix64 Parm21 = (DFix64)1;
        public DFix64 Parm22 = (DFix64)0.95f;
        public DFix64 Parm23 = (DFix64)1.05f;
        public DFix64 Parm24 = (DFix64)0;
        public DFix64 Parm25 = (DFix64)100;
        public DFix64 Parm26 = (DFix64)0;
        public DFix64 Parm27 = (DFix64)100;
        public DFix64 Parm28 = (DFix64)0;
        public DFix64 Parm29 = (DFix64)100;
        public DFix64 Parm30 = (DFix64)0.25f;
        public DFix64 Parm31 = (DFix64)1;
        public DFix64 Parm32 = (DFix64)0;
        public DFix64 Parm33 = (DFix64)100;
        public DFix64 Parm34 = (DFix64)0.25f;
        public DFix64 Parm35 = (DFix64)100;
        public DFix64 Parm36 = (DFix64)0;
        public DFix64 Parm37 = (DFix64)0;
        public DFix64 Parm38 = (DFix64)100;
        public DFix64 Parm39 = (DFix64)0;
        public DFix64 Parm40 = (DFix64)100;
        public DFix64 Parm41 = (DFix64)0;
        public DFix64 Parm42 = (DFix64)0;
        public DFix64 Parm43 = (DFix64)100;
        public DFix64 Parm44 = (DFix64)0;
        public DFix64 Parm45 = (DFix64)0.25f;
        public DFix64 Parm46 = (DFix64)10;
        public DFix64 Parm47 = (DFix64)400f;


        public void Load()
        {
            Parm15 = LoadParm(15, (DFix64)0.4f);
            Parm16 = LoadParm(16, (DFix64)200);
            Parm17 = LoadParm(17, (DFix64)5000);
            Parm18 = LoadParm(18, (DFix64)1);
            Parm19 = LoadParm(19, (DFix64)100);
            Parm20 = LoadParm(20, (DFix64)0);
            Parm21 = LoadParm(21, (DFix64)1);
            Parm22 = LoadParm(22, (DFix64)0.95f);
            Parm23 = LoadParm(23, (DFix64)1.05f);
            Parm24 = LoadParm(24, (DFix64)0);
            Parm25 = LoadParm(25, (DFix64)100);
            Parm26 = LoadParm(26, (DFix64)0);
            Parm27 = LoadParm(27, (DFix64)100);
            Parm28 = LoadParm(28, (DFix64)0);
            Parm29 = LoadParm(29, (DFix64)100);
            Parm30 = LoadParm(30, (DFix64)0.25f);
            Parm31 = LoadParm(31, (DFix64)1);
            Parm32 = LoadParm(32, (DFix64)0);
            Parm33 = LoadParm(33, (DFix64)100);
            Parm34 = LoadParm(34, (DFix64)0.25f);
            Parm35 = LoadParm(35, (DFix64)100);
            Parm36 = LoadParm(36, (DFix64)0);
            Parm37 = LoadParm(37, (DFix64)0);
            Parm38 = LoadParm(38, (DFix64)100);
            Parm39 = LoadParm(39, (DFix64)0);
            Parm40 = LoadParm(40, (DFix64)100);
            Parm41 = LoadParm(41, (DFix64)0);
            Parm42 = LoadParm(42, (DFix64)0);
            Parm43 = LoadParm(43, (DFix64)100);
            Parm44 = LoadParm(44, (DFix64)0);
            Parm45 = LoadParm(45, (DFix64)0.25f);
            Parm46 = LoadParm(46, (DFix64)10);
            Parm46 = LoadParm(47, (DFix64)400f);
        }

        private DFix64 LoadParm(int id, DFix64 defaultValue)
        {
            DRBasicParm drBasicParm = GameManager.DataTable.GetDataRow<DRBasicParm>(id);
            if (drBasicParm != null && !string.IsNullOrEmpty(drBasicParm.ParmValueText))
            {
                float value;
                if (float.TryParse(drBasicParm.ParmValueText, out value))
                {
                    return (DFix64)value;
                }
            }

            return defaultValue;
        }
    }
}