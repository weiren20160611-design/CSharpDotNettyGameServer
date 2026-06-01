using System;
using System.Linq;
using System.Text;

namespace Game.Datas.DBEntities
{
    ///<summary>
    ///
    ///</summary>
    public partial class Emailmessage
    {
           public Emailmessage(){


           }
           /// <summary>
           /// Desc:消息唯一ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long id {get;set;}

           /// <summary>
           /// Desc:消息来自哪个玩家的ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? fromPlayerId {get;set;}

           /// <summary>
           /// Desc:发送给哪个玩家的ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? toPlayerId {get;set;}

           /// <summary>
           /// Desc:消息数据
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string msgBody {get;set;}

           /// <summary>
           /// Desc:消息状态0:未读,1:已读,2:删除
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? status {get;set;}

           /// <summary>
           /// Desc:发送时间
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public long sendTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public long? userData {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? readTime {get;set;}

    }
}
