//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MESACCA
{
    using System;
    using System.Collections.Generic;
    
    public partial class NewsArticle
    {
        public int ArticleID { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleBody { get; set; }
        public int CreatedByUser { get; set; }
        public System.DateTime DateOfArticle { get; set; }
    
        public virtual UserTable User { get; set; }
    }
}