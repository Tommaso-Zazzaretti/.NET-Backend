using Microservice.Application.Services.Linq.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace Microservice.Application.Services.Linq
{
    public class LinqBuilderService : ILinqBuilderService
    {
        public readonly IDictionary<Type, Dictionary<string, PropertyInfo>> _propertiesMetadata;
        public readonly IDictionary<Type, Dictionary<string, MethodInfo>>   _methodsMetadata;

        public LinqBuilderService(Assembly assembly,string NameSpace) {
            this._propertiesMetadata = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            this._methodsMetadata    = new Dictionary<Type, Dictionary<string, MethodInfo>>();
            this.Init(assembly, NameSpace);
        }

        private void Init(Assembly assembly, string NameSpace) {
            //PROPERTIES DICTIONARY
            IEnumerable<Type> Types = assembly.GetTypes().Where(type => string.Equals(NameSpace,type.Namespace,StringComparison.Ordinal));
            foreach (var TypeObj in Types) {
                this._propertiesMetadata[TypeObj] = new Dictionary<string, PropertyInfo>();
                IEnumerable<PropertyInfo> Properties = TypeObj.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (PropertyInfo PropertyObj in Properties) {
                    this._propertiesMetadata[TypeObj][PropertyObj.Name] = PropertyObj;
                }
            }
            //METHODS DICTIONARY (UTILS)  string methods => for like search
            this._methodsMetadata[typeof(string)] = new Dictionary<string, MethodInfo>()
            {
                ["Contains"]   = typeof(string).GetMethod("Contains"  , new[] { typeof(string) })!,
                ["StartsWith"] = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!
            };
        }

        public Expression<Func<T, dynamic>> Selector<T>(string PropertyName) where T : class 
        {
            PropertyInfo PropertyObj = this._propertiesMetadata[typeof(T)][PropertyName];
            var ExprArgument = Expression.Parameter(typeof(T), "elem");             // elem =>
            var ExprProperty = Expression.Property(ExprArgument, PropertyObj.Name); //      => TObj.PName
            var CastProperty = Expression.Convert(ExprProperty, typeof(object));    //      => Convert(TObj.PName,Object)
            return Expression.Lambda<Func<T, dynamic>>(CastProperty, ExprArgument); // elem => Convert(elem.PName,Object)
        }

        public Expression<Func<T, bool>> StringPredicate<T>(string StringPropertyName, string MethodName, string Pattern) where T : class 
        {
            PropertyInfo PropertyObj = this._propertiesMetadata[typeof(T)][StringPropertyName];
            if(PropertyObj.PropertyType != typeof(string)) { throw new Exception("The StringPropertyName is not a string property."); } //ONLY FOR STRING FIELDS
            var StringMethodObj = this._methodsMetadata[typeof(string)][MethodName];
            var ExprArgument = Expression.Parameter(typeof(T), "elem");                       // elem =>
            var ExprProperty = Expression.Property(ExprArgument, PropertyObj.Name);           //      => TObj.PName
            var ExprConstant = Expression.Constant(Pattern, typeof(string));                  // "<Pattern>"
            var ExprCalling  = Expression.Call(ExprProperty, StringMethodObj, ExprConstant);  // elem => elem.PName.StringMethod("<Pattern>")
            return Expression.Lambda<Func<T, bool>>(ExprCalling, ExprArgument);
        }

        public Expression<Func<T, bool>> EqualityPredicate<T,TConst>(string StringPropertyName, TConst Constant) where T : class
        {
            PropertyInfo PropertyObj = this._propertiesMetadata[typeof(T)][StringPropertyName];
            if (PropertyObj.PropertyType != typeof(TConst)) { throw new Exception("The StringPropertyName type is not the Constant type"); } //ONLY FOR STRING FIELDS
            var ExprArgument = Expression.Parameter(typeof(T), "elem");               // elem =>
            var ExprProperty = Expression.Property(ExprArgument, PropertyObj.Name);   //      => TObj.PName
            var ExprConstant = Expression.Constant(Constant, typeof(TConst));         // <Const>
            var ExprEquality = Expression.Equal(ExprProperty, ExprConstant);          // elem => elem.Equals(<Const>)
            return Expression.Lambda<Func<T, bool>>(ExprEquality, ExprArgument);
        }
    }
}
