using Direwolf.Dto.Driver;
using Direwolf.Dto.InternalDb.Enums;
using Direwolf.Dto.Parser;

namespace Direwolf.Contracts;

public interface IWolfpack
{
    public DocumentType DocumentType { get; set; }
    public Realm Realm { get; set; }
    public List<Set> QuerySets { get; set; }
    public List<DriverCommonDto> Drivers { get; set; }
}

/*
  # file example
wolfpack:
   document_type: document
   platform: desktop
   queries:
       get_doors:
           query_1:
               execution_trigger: none
               operations:
                - get_doors:
                    - doors:
                      scope: category
                      p: '*'
                      op: OfCategory
                      q: OST_Doors

                    - name:
                      scope: parameter
                      p: name
                      op: equals
                      q: 'Door_Wood'

                - check_for_conditions:
                  - width:
                    scope: parameter
                    p: 'Width'
                    op: equals
                    q: '200'

                  - height:
                    scope: parameter
                    p: 'Height'
                    op: greater_than
                    q: '255'

                - join_both_conditions:
                    scope: query_1
                    p: get_doors
                    op: and
                    q: check_for_conditions
       set_2:
           query_2:
               # another set of queries
       operation:
           p: set_1
           op: join
           q: set_2
   drivers:
       json_file:
           path: "./result.json"

 */