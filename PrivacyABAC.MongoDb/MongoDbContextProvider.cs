using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PrivacyABAC.DbInterfaces.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivacyABAC.MongoDb
{
    public class MongoDbContextProvider
    {
        public string ConnectionString { get; set; }

        public string PolicyDatabaseName { get; set; }

        public string UserDatabaseName { get; set; }

        public string UserCollectionName { get; set; }

        public static void Setup()
        {
            BsonClassMap.RegisterClassMap<AccessControlPolicy>(p => {
                p.MapMember(c => c.Id).SetElementName("_id").SetSerializer(new StringSerializer(BsonType.ObjectId));
                p.MapMember(c => c.CollectionName).SetElementName("collection_name");
                p.MapMember(c => c.Description).SetElementName("description");
                p.MapMember(c => c.RuleCombining).SetElementName("rule_combining");
                p.MapMember(c => c.IsAttributeResourceRequired).SetElementName("isAttributeResourceRequired");
                p.MapMember(c => c.Action).SetElementName("action");
                p.MapMember(c => c.Target).SetElementName("target");
                p.MapMember(c => c.Rules).SetElementName("rules");
                p.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<PrivacyPolicy>(p => {
                p.MapMember(c => c.Id).SetElementName("_id").SetSerializer(new StringSerializer(BsonType.ObjectId));
                p.MapMember(c => c.CollectionName).SetElementName("collection_name");
                p.MapMember(c => c.Description).SetElementName("description");
                p.MapMember(c => c.IsAttributeResourceRequired).SetElementName("is_attribute_resource_required");
                p.MapMember(c => c.Target).SetElementName("target");
                p.MapMember(c => c.Rules).SetElementName("rules");
                p.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<AccessControlRule>(p => {
                p.MapMember(c => c.Id).SetElementName("_id");
                p.MapMember(c => c.Effect).SetElementName("effect");
                p.MapMember(c => c.Condition).SetElementName("condition");
                p.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Function>(p => {
                p.MapMember(c => c.FunctionName).SetElementName("function_name");
                p.MapMember(c => c.Parameters).SetElementName("parameters");
                p.MapMember(c => c.Value).SetElementName("value");
                p.MapMember(c => c.ResourceID).SetElementName("resource_id");
                p.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<FieldRule>(p => {
                p.MapMember(c => c.FieldEffects).SetElementName("field_effects");
                p.MapMember(c => c.Condition).SetElementName("condition");
                p.MapMember(c => c.Identifer).SetElementName("identifier");
                p.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<FieldEffect>(p => {
                p.MapMember(c => c.Name).SetElementName("name");
                p.MapMember(c => c.FunctionApply).SetElementName("effect_function");
                p.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<PrivacyDomain>(p => {
                p.MapMember(c => c.Id).SetElementName("_id").SetSerializer(new StringSerializer(BsonType.ObjectId));
                p.MapMember(c => c.DomainName).SetElementName("domain_name");
                p.MapMember(c => c.IsArrayFieldDomain).SetElementName("is_sub_policy");
                p.MapMember(c => c.Fields).SetElementName("fields");
                p.MapMember(c => c.Functions).SetElementName("hierarchy");
                p.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<PriorityFunction>(p => {
                p.MapMember(c => c.Name).SetElementName("name");
                p.MapMember(c => c.Priority).SetElementName("priority");
                p.SetIgnoreExtraElements(true);
            });
        }
    }
}
