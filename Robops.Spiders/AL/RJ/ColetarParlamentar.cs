using RafaelEstevam.Simple.Spider.Helper;
using Robops.Lib.AL.RJ.ApiModels;
using System;

namespace Robops.Spiders.AL.RJ
{
    public class ColetarParlamentar
    {
        public static CongressmanData.Congressman[] ObterParlamentares()
        {
            // url já sem paginação
            var uri = new Uri("https://docigp.alerj.rj.gov.br/api/v1/congressmen?query=%7B%22filter%22:%7B%22text%22:null,%22checkboxes%22:%7B%22withMandate%22:false,%22withoutMandate%22:false,%22withPendency%22:false,%22withoutPendency%22:false,%22unread%22:false,%22joined%22:true,%22notJoined%22:false,%22filler%22:false%7D,%22selects%22:%7B%22filler%22:false%7D%7D,%22pagination%22:%7B%22total%22:83,%22per_page%22:%22250%22,%22current_page%22:1,%22last_page%22:9,%22from%22:1,%22to%22:10,%22pages%22:[1,2,3,4,5]%7D%7D");
            var registros = FetchHelper.FetchResourceJson<CongressmanData>(uri);

            return registros.data;
        }
    }
}
