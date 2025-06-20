// using Direwolf.Definitions.Enums;
// using Direwolf.Definitions.LLM;
//
// namespace Direwolf.Definitions;
//
//     
//     {
//         public static Wolfpack Create(Cuid? id, 
//                 MessageType requestType, 
//                 string? name,
//                 WolfpackParams @params, 
//                 string? description = null)
//             {
//                 return new Wolfpack(id ?? Cuid.Create(), 
//                     requestType, 
//                     name, 
//                     @params, 
//                     description);
//             }
//         
//             public static Wolfpack Create(MessageType requestType, 
//                 string? name, 
//                 WolfpackParams @params,
//                 string? description = null)
//             {
//                 return new Wolfpack(Cuid.Create(), 
//                     requestType, 
//                     name, 
//                     @params, 
//                     description);
//             }
//     }
