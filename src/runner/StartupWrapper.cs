using System;
using System.Linq;
using System.Reflection;

namespace OpenFaaS
{
    internal class StartupWrapper
    {
        private readonly Type instanceType;
        private readonly object instance;

        private StartupWrapper( Type instanceType, object instance )
        {
            this.instanceType = instanceType;
            this.instance = instance;
        }

        public bool InvokeIfExists( string methodName, object[] args )
        {
            if ( instance == null )
            {
                return ( false );
            }

            var types = ( args ?? Enumerable.Empty<object>() )
                .Select( x => x.GetType() )
                .ToArray();

            var method = instance.GetType().GetMethod( methodName
                , BindingFlags.Public | BindingFlags.Instance
                , null
                , types
                , null );

            if ( method == null )
            {
                return ( false );
            }

            method.Invoke( instance, args );

            return ( true );
        }
    
        public static StartupWrapper TryCreate( Type type, Microsoft.Extensions.Configuration.IConfiguration configuration )
        {
            if ( type == null )
            {
                return ( null );
            }

            // lookup constructor without parameters (minimal model)
            var ctor = type.GetConstructor( BindingFlags.Public, System.Type.EmptyTypes );

            if ( ctor != null )
            {
                return TryCreate( type, ctor, null );
            }

            // if that doesn't work
            // lookup constructor with configuration argument (Generic Host model)
            ctor = type.GetConstructor( BindingFlags.Public, new Type[]
            {
                typeof( Microsoft.Extensions.Configuration.IConfiguration )
            } );

            if ( ctor != null )
            {
                return TryCreate( type, ctor, new object[]
                {
                    configuration
                } );
            }

            // no suitable constructor
            return ( null );
        }

        private static StartupWrapper TryCreate( Type type, ConstructorInfo ctor, object[] args )
        {
            var instance = ctor.Invoke( args );

            if ( instance == null )
            {
                return ( null );
            }

            return new StartupWrapper( type, instance );
        }
    }
}
