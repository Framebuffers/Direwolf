using Direwolf.Parsers;
using Direwolf.Parsers.Tokens;

namespace Direwolf.Contracts;

public interface IWolfpack
{
    public DocumentType DocumentType { get; set; }
    public PlatformType Platform { get; set; }
    public List<Set> QuerySets { get; set; }
    public List<Driver> Drivers { get; set; }
}

/*
 * # file example
 * wolfpack:
 *      document_type: document
 *      platform: desktop
 *      queries:
 *          set_1:
 *              query_1:
 *                  execution_trigger: none
 *                  target: 
 *                  query_scope: category
 *                      p: *
 *                      op: OfCategory
 *                      q: OST_Walls
 *                  query_scope: family
 *                      operation_1:
 *                          p: name
 *                          op: equals
 *                          q: 'Door_Wood'
 *                  query_scope: parameter
 *                      operation_1:
 *                          p: 'Width'
 *                          op: equals
 *                          q: '200'
 *                      operation_2:
 *                          p: 'Height'
 *                          op: greater_than
 *                          q: '255'
 *                      operation_3:
 *                          p: operation_1
 *                          op: and
 *                          q: operation_2
 *          set_2:
 *              query_2:
 *                  # another set of queries
 *          operation:
 *              p: set_1
 *              op: join
 *              q: set_2
 *      drivers:
 *          json_file:
 *              path: "./result.json"
 *              indented: true
 */

