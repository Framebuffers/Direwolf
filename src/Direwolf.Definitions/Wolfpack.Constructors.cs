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
//                 WolfpackMessage @Properties, 
//                 string? description = null)
//             {
//                 return new Wolfpack(id ?? Cuid.Create(), 
//                     requestType, 
//                     name, 
//                     @Properties, 
//                     description);
//             }
//         
//             public static Wolfpack Create(MessageType requestType, 
//                 string? name, 
//                 WolfpackMessage @Properties,
//                 string? description = null)
//             {
//                 return new Wolfpack(Cuid.Create(), 
//                     requestType, 
//                     name, 
//                     @Properties, 
//                     description);
//             }
//     }
