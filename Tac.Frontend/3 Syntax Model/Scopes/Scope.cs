//using System;

//namespace Tac.Semantic_Model
//{

//    //why does this live here?
//    // where is the right place for it to live??
//    // this is really owned by what scope it is on right?
//    public enum DefintionLifetime
//    {
//        Static,
//        Instance,
//    }

//    public class Visiblity<TReferanced> where TReferanced : class
//    {
//        public Visiblity(DefintionLifetime defintionLifeTime, TReferanced definition)
//        {
//            DefintionLifeTime = defintionLifeTime;
//            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
//        }

//        public DefintionLifetime DefintionLifeTime { get; }
//        public TReferanced Definition { get; }
//    }
//}