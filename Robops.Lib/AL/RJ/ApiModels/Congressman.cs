using System;

namespace Robops.Lib.AL.RJ.ApiModels
{
    public class CongressmanData
    {
        public int current_page { get; set; }
        public Congressman[] data { get; set; }
        public string first_page_url { get; set; }
        public int from { get; set; }
        public int last_page { get; set; }
        public string last_page_url { get; set; }
        public Link[] links { get; set; }
        public string next_page_url { get; set; }
        public string path { get; set; }
        public int per_page { get; set; }
        public string prev_page_url { get; set; }
        public int to { get; set; }
        public int total { get; set; }

        public class Congressman
        {
            public int id { get; set; }
            public int remote_id { get; set; }
            public string name { get; set; }
            public string nickname { get; set; }
            public int party_id { get; set; }
            public string photo_url { get; set; }
            public string thumbnail_url { get; set; }
            public int department_id { get; set; }
            public int created_by_id { get; set; }
            public object updated_by_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public bool has_mandate { get; set; }
            public bool has_pendency { get; set; }
            public bool is_published { get; set; }
            public string thumbnail_url_linkable { get; set; }
            public string photo_url_linkable { get; set; }
            public Party party { get; set; }
            public User user { get; set; }
        }

        public class Party
        {
            public int id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public int created_by_id { get; set; }
            public int? updated_by_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class User
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public object email_verified_at { get; set; }
            public int per_page { get; set; }
            public object created_by_id { get; set; }
            public object updated_by_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public string username { get; set; }
            public int department_id { get; set; }
            public int congressman_id { get; set; }
            public string last_login_at { get; set; }
        }

        public class Link
        {
            public string url { get; set; }
            public string label { get; set; }
            public bool active { get; set; }
        }
    }
}
