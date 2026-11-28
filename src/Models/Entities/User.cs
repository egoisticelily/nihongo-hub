using LiteDB;

namespace NihongoHub.Models.Entities
{
    /// <summary>
    /// 用户权限为 admin 与 user(普通用户)两种(以后可能会新增)
    /// admin 可以管理其他用户: 删除用户、修改密码、启停用户等
    /// admin 同样可以使用全部功能,不是"管理专用"账号
    /// 现阶段只允许一个 admin,后续可能会修改
    /// 其他表中的 userid 与这里的 id 一致,由创建时保证
    /// </summary>
    
    public enum UserType
    {
        user = 0,
        admin = 1
    }

    public class User
    {
        [BsonId]
        public int id {get;set;}
        public required string username {get;set;}
        public required string passwordhash {get;set;}
        public required bool enabled {get;set;}
        public required UserType usertype {get;set;}
    }
}