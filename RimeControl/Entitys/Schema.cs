using System.ComponentModel;

namespace RimeControl.Entitys
{
    //输入法方案
    public class Schema : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 使用中
        /// </summary>
        public bool isUsing { get; set; }

        private bool _isSelect;

        /// <summary>
        /// 勾选
        /// </summary>
        public bool isSelect
        {
            get { return _isSelect; }
            set {
                _isSelect = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("isSelect"));
                }
            }
        }


        /// <summary>
        /// 预设方案
        /// </summary>
        public bool isSys { get; set; }
        /// <summary>
        /// schema id
        /// </summary>
        public string schema_id { get; set; }
        /// <summary>
        /// schema 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// schema 版本信息
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// schema 作者信息
        /// </summary>
        public string author { get; set; }
        /// <summary>
        /// schema描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 依赖的 dependencies  
        /// 依赖的Schema Id数组
        /// </summary>
        public string dependencies { get; set; }
        /// <summary>
        /// 是否在Roaming\Rime目录
        /// </summary>
        public bool inRoaming { get; set; }
    }
}
