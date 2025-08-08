# DontPanicLabs.Ifx.Proxy.Contracts

The foundational contracts and base implementations for the Don't Panic Labs Infrastructure Framework (Ifx) proxy system. This package provides essential interfaces, abstract classes, and core functionality needed to build context-aware, interceptor-enabled service proxies.

## Overview

This package serves as the cornerstone for creating dynamic service proxies that support:

- **Context-aware service resolution**: Services are resolved with appropriate request/ambient context
- **Interception capabilities**: Support for method interception using Castle DynamicProxy
- **Service lifecycle management**: Base classes for proxy-enabled services with proper dependency injection
- **Extensible proxy generation**: Core interfaces that can be implemented by different IoC container adapters

### Key Components

- **Service Interfaces**: `IProxyEnabledService`, `IProxyEnabledComponent`, `IProxyEnabledSubsystem`, `IProxyEnabledUtility`
- **Base Classes**: `ProxyEnabledServiceBase` for implementing proxy-enabled services
- **Proxy Generation**: `IProxyGenerator` interface for creating service proxies
- **Context Management**: Base classes for ambient and request-scoped contexts
- **Exception Handling**: Custom exceptions for proxy-related errors

## Getting started

This package should be automatically added as a dependency of other Don't Panic Labs Proxy packages.

## Feedback

Submit issues at:

https://github.com/dontpaniclabs/DontPanicLabs.Ifx/issues