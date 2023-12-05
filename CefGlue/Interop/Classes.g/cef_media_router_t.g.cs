﻿//
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
//
namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Security;
    
    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
    internal unsafe struct cef_media_router_t
    {
        internal cef_base_ref_counted_t _base;
        internal IntPtr _add_observer;
        internal IntPtr _get_source;
        internal IntPtr _notify_current_sinks;
        internal IntPtr _create_route;
        internal IntPtr _notify_current_routes;
        
        // GetGlobalMediaRouter
        [DllImport(libcef.DllName, EntryPoint = "cef_media_router_get_global", CallingConvention = libcef.CEF_CALL)]
        public static extern cef_media_router_t* get_global(cef_completion_callback_t* callback);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate void add_ref_delegate(cef_media_router_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int release_delegate(cef_media_router_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int has_one_ref_delegate(cef_media_router_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int has_at_least_one_ref_delegate(cef_media_router_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_registration_t* add_observer_delegate(cef_media_router_t* self, cef_media_observer_t* observer);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_media_source_t* get_source_delegate(cef_media_router_t* self, cef_string_t* urn);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate void notify_current_sinks_delegate(cef_media_router_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate void create_route_delegate(cef_media_router_t* self, cef_media_source_t* source, cef_media_sink_t* sink, cef_media_route_create_callback_t* callback);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate void notify_current_routes_delegate(cef_media_router_t* self);
        
        // AddRef
        private static IntPtr _p0;
        private static add_ref_delegate _d0;
        
        public static void add_ref(cef_media_router_t* self)
        {
            add_ref_delegate d;
            var p = self->_base._add_ref;
            if (p == _p0) { d = _d0; }
            else
            {
                d = (add_ref_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(add_ref_delegate));
                if (_p0 == IntPtr.Zero) { _d0 = d; _p0 = p; }
            }
            d(self);
        }
        
        // Release
        private static IntPtr _p1;
        private static release_delegate _d1;
        
        public static int release(cef_media_router_t* self)
        {
            release_delegate d;
            var p = self->_base._release;
            if (p == _p1) { d = _d1; }
            else
            {
                d = (release_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(release_delegate));
                if (_p1 == IntPtr.Zero) { _d1 = d; _p1 = p; }
            }
            return d(self);
        }
        
        // HasOneRef
        private static IntPtr _p2;
        private static has_one_ref_delegate _d2;
        
        public static int has_one_ref(cef_media_router_t* self)
        {
            has_one_ref_delegate d;
            var p = self->_base._has_one_ref;
            if (p == _p2) { d = _d2; }
            else
            {
                d = (has_one_ref_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(has_one_ref_delegate));
                if (_p2 == IntPtr.Zero) { _d2 = d; _p2 = p; }
            }
            return d(self);
        }
        
        // HasAtLeastOneRef
        private static IntPtr _p3;
        private static has_at_least_one_ref_delegate _d3;
        
        public static int has_at_least_one_ref(cef_media_router_t* self)
        {
            has_at_least_one_ref_delegate d;
            var p = self->_base._has_at_least_one_ref;
            if (p == _p3) { d = _d3; }
            else
            {
                d = (has_at_least_one_ref_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(has_at_least_one_ref_delegate));
                if (_p3 == IntPtr.Zero) { _d3 = d; _p3 = p; }
            }
            return d(self);
        }
        
        // AddObserver
        private static IntPtr _p4;
        private static add_observer_delegate _d4;
        
        public static cef_registration_t* add_observer(cef_media_router_t* self, cef_media_observer_t* observer)
        {
            add_observer_delegate d;
            var p = self->_add_observer;
            if (p == _p4) { d = _d4; }
            else
            {
                d = (add_observer_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(add_observer_delegate));
                if (_p4 == IntPtr.Zero) { _d4 = d; _p4 = p; }
            }
            return d(self, observer);
        }
        
        // GetSource
        private static IntPtr _p5;
        private static get_source_delegate _d5;
        
        public static cef_media_source_t* get_source(cef_media_router_t* self, cef_string_t* urn)
        {
            get_source_delegate d;
            var p = self->_get_source;
            if (p == _p5) { d = _d5; }
            else
            {
                d = (get_source_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_source_delegate));
                if (_p5 == IntPtr.Zero) { _d5 = d; _p5 = p; }
            }
            return d(self, urn);
        }
        
        // NotifyCurrentSinks
        private static IntPtr _p6;
        private static notify_current_sinks_delegate _d6;
        
        public static void notify_current_sinks(cef_media_router_t* self)
        {
            notify_current_sinks_delegate d;
            var p = self->_notify_current_sinks;
            if (p == _p6) { d = _d6; }
            else
            {
                d = (notify_current_sinks_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(notify_current_sinks_delegate));
                if (_p6 == IntPtr.Zero) { _d6 = d; _p6 = p; }
            }
            d(self);
        }
        
        // CreateRoute
        private static IntPtr _p7;
        private static create_route_delegate _d7;
        
        public static void create_route(cef_media_router_t* self, cef_media_source_t* source, cef_media_sink_t* sink, cef_media_route_create_callback_t* callback)
        {
            create_route_delegate d;
            var p = self->_create_route;
            if (p == _p7) { d = _d7; }
            else
            {
                d = (create_route_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(create_route_delegate));
                if (_p7 == IntPtr.Zero) { _d7 = d; _p7 = p; }
            }
            d(self, source, sink, callback);
        }
        
        // NotifyCurrentRoutes
        private static IntPtr _p8;
        private static notify_current_routes_delegate _d8;
        
        public static void notify_current_routes(cef_media_router_t* self)
        {
            notify_current_routes_delegate d;
            var p = self->_notify_current_routes;
            if (p == _p8) { d = _d8; }
            else
            {
                d = (notify_current_routes_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(notify_current_routes_delegate));
                if (_p8 == IntPtr.Zero) { _d8 = d; _p8 = p; }
            }
            d(self);
        }
        
    }
}
