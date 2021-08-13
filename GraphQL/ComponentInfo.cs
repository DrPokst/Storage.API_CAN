using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQLRequests.GraphQL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLRequests.GraphQL
{
    public class ComponentInfo
    {
        private readonly IGraphQLClient _client;
        public ComponentInfo(IGraphQLClient client)
        {
            _client = client;
        }

        public async Task<GraphQLData> GetAllComponentInfo(string mnf, int set, string currency)
        {
            var query = new GraphQLRequest
            {
                Query = @"
 query MyPartSearch($q: String!, $limit: Int!, $currency: String!) {
   search(q: $q, limit: $limit, currency: $currency) {
     results {
      part {
        mpn
        category{
          name
        }
        manufacturer {
          name
        }
        manufacturer_url
        best_datasheet {
          name
          url
        }
        short_description
        octopart_url
        images{
          url
        }
        specs {
          attribute {
            name
            group
          }
          display_value
        }
        sellers(include_brokers: false, authorized_only: true) {
          company {
            name
          }
          offers {
            sku
            moq
            click_url
            updated
            inventory_level
            prices {
              price
              currency
              quantity
              converted_price
              converted_currency
            }
          }
        }
      }
    }
                        }
                      }",
                Variables = new { 
                    q = mnf, 
                    limit = set, 
                    currency = currency }
            };



            var response = await _client.SendQueryAsync<GraphQLData>(query);
          
            return response.Data;
        }
    }
}
