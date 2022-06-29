using System;

namespace Robops.Lib.AL.RJ.ApiModels
{
    public class BudgetInfo
    {
        public int current_page { get; set; }
        public DataReferencia[] data { get; set; }
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


        public class DataReferencia
        {
            public int id { get; set; }
            public int congressman_legislature_id { get; set; }
            public int budget_id { get; set; }
            public string percentage { get; set; }
            public string value { get; set; }
            public int analysed_by_id { get; set; }
            public string analysed_at { get; set; }
            public int published_by_id { get; set; }
            public string published_at { get; set; }
            public int created_by_id { get; set; }
            public int updated_by_id { get; set; }
            public int? transport_from_previous_entry_id { get; set; }
            public int? transport_to_next_entry_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public int closed_by_id { get; set; }
            public string closed_at { get; set; }
            public bool missing_analysis { get; set; }
            public bool missing_verification { get; set; }
            public bool has_deposit { get; set; }
            public int entries_count { get; set; }
            public string sum_credit { get; set; }
            public string sum_debit { get; set; }
            public object sum_i { get; set; }
            public object sum_ii { get; set; }
            public string sum_iii { get; set; }
            public string sum_iv { get; set; }
            public object sum_v { get; set; }
            public string sum_vi_a { get; set; }
            public string sum_vi_b { get; set; }
            public string sum_vii { get; set; }
            public string sum_viii { get; set; }
            public object sum_ix { get; set; }
            public string sum_x { get; set; }
            public string sum_xi { get; set; }
            public bool has_refund { get; set; }
            public Budget budget { get; set; }
            public Congressman_Legislature congressman_legislature { get; set; }
            public Entry[] entries { get; set; }
            public int year { get; set; }
            public string month { get; set; }
            public string state_value_formatted { get; set; }
            public string value_formatted { get; set; }
            public string sum_debit_formatted { get; set; }
            public string sum_credit_formatted { get; set; }
            public int balance { get; set; }
            public string balance_formatted { get; set; }
            public string percentage_formatted { get; set; }
            public object[] pendencies { get; set; }
        }

        public class Budget
        {
            public int id { get; set; }
            public DateTime date { get; set; }
            public string federal_value { get; set; }
            public string percentage { get; set; }
            public string value { get; set; }
            public int created_by_id { get; set; }
            public int updated_by_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class Congressman_Legislature
        {
            public int id { get; set; }
            public int congressman_id { get; set; }
            public int legislature_id { get; set; }
            public DateTime started_at { get; set; }
            public object ended_at { get; set; }
            public int created_by_id { get; set; }
            public object updated_by_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public Congressman congressman { get; set; }
        }

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
            public object updated_by_id { get; set; }
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

        public class Entry
        {
            public int id { get; set; }
            public DateTime date { get; set; }
            public decimal value { get; set; }
            public int cost_center_id { get; set; }
            public int congressman_budget_id { get; set; }
            public string @object { get; set; }
            public string to { get; set; }
            public int entry_type_id { get; set; }
            public string document_number { get; set; }
            public int provider_id { get; set; }
            public DateTime verified_at { get; set; }
            public int verified_by_id { get; set; }
            public DateTime analysed_at { get; set; }
            public int analysed_by_id { get; set; }
            public DateTime published_at { get; set; }
            public int published_by_id { get; set; }
            public DateTime created_at { get; set; }
            public int created_by_id { get; set; }
            public DateTime updated_at { get; set; }
            public int updated_by_id { get; set; }
            public bool is_transport { get; set; }
            public bool is_transport_or_credit { get; set; }
            public int comments_count { get; set; }
            public bool provider_is_blocked { get; set; }
            public Provider provider { get; set; }
        }

        public class Provider
        {
            public int id { get; set; }
            public string cpf_cnpj { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public int created_by_id { get; set; }
            public int? updated_by_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public object zipcode { get; set; }
            public object street { get; set; }
            public object number { get; set; }
            public object complement { get; set; }
            public object neighborhood { get; set; }
            public object city { get; set; }
            public object state { get; set; }
            public object fullAddress { get; set; }
            public bool is_blocked { get; set; }
        }

        public class Link
        {
            public string url { get; set; }
            public string label { get; set; }
            public bool active { get; set; }
        }
    }
}
