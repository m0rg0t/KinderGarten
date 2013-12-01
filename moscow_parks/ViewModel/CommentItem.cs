using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moscow_parks.ViewModel
{
    public class CommentItem: ViewModelBase
    {
        public CommentItem() {
        }

        private int _id;
        /// <summary>
        /// Идентификатор комментария
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _username;
        /// <summary>
        /// 
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _comment;
        /// <summary>
        /// 
        /// </summary>
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        private DateTime _createdAt;
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedAt
        {
            get { return _createdAt; }
            set { _createdAt = value; }
        }

        private string _parkId;
        /// <summary>
        /// unique region code
        /// </summary>
        public string ParkId
        {
            get { return _parkId; }
            set { _parkId = value; }
        }
        
        
        
        
    }
}
