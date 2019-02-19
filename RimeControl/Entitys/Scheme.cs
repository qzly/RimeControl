using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimeControl.Entitys
{
    /// <summary>
    /// 绑定 list的类
    /// </summary>
    public class Scheme: INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public string id { get; set; }

        //name需要使用通知机制，通知ListBox数据发生改变了
        //参考 https://bbs.csdn.net/topics/390883049 #2的回答
        private string _name;
        public string name {
            get { return _name; }
            set
            {
                _name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("name"));
                }
            }
        }

        public string author { get; set; }

        public System.Windows.Media.Color back_color { get; set; }
        public System.Windows.Media.Color border_color { get; set; }

        public System.Windows.Media.Color text_color { get; set; }
        public System.Windows.Media.Color hilited_text_color { get; set; }
        public System.Windows.Media.Color hilited_back_color { get; set; }

        public System.Windows.Media.Color hilited_candidate_back_color { get; set; }
        public System.Windows.Media.Color hilited_candidate_text_color { get; set; }

        public System.Windows.Media.Color candidate_text_color { get; set; }
        public System.Windows.Media.Color comment_text_color { get; set; }
        
        /// <summary>
        /// 是否自带皮肤
        /// </summary>
        public bool isSysScheme { get; set; }
        /// <summary>
        /// 是否使用中皮肤
        /// </summary>
        public bool isUsing { get; set; }
        /// <summary>
        /// 是否新添加
        /// </summary>
        public bool isNew { get; set; }


        public Scheme Clone()
        {
            Scheme newScheme = new Scheme
            {
                id = this.id,

                name = this.name,
                author = this.author,

                back_color = this.back_color,
                border_color = this.border_color,

                text_color = this.text_color,
                hilited_text_color = this.hilited_text_color,
                hilited_back_color = this.hilited_back_color,

                hilited_candidate_back_color = this.hilited_candidate_back_color,
                hilited_candidate_text_color = this.hilited_candidate_text_color,

                candidate_text_color = this.candidate_text_color,
                comment_text_color = this.comment_text_color,
            };
            return newScheme;
        }
    }

}
