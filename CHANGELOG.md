# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="2.0.1"></a>
## 2.0.1 (2024-11-22)

### Bug Fixes

* **jsonschemaprovider:** added a provider for json schema resolving

<a name="2.0.0"></a>
## 2.0.0 (2024-11-12)

### Features

* added resourcetype service
* added schemaService for retrieving SCIM api resources schemas
* **bulkoperation:** added implementation of bulkoperation service
* **bulkoperations:** added route for bulk operations
* **bulkservice:** added some validations and added the execution of post operation
* **schemaroutes:** added middleware to use schema routes
* **scimv2:** added pagination middleware, added bulk operation objects
* **scimv2:** added serverproviderconfiguration route

### Bug Fixes

* fix route names
* **jsonutils:** moved to jsonutils on latest version of core package
* **scimv2routeoptionstests:** fixed method not allowd for put test case

### Breaking Changes

* added schemaService for retrieving SCIM api resources schemas
* **scimv2:** added pagination middleware, added bulk operation objects

<a name="1.4.4"></a>
## 1.4.4 (2024-09-24)

### Bug Fixes

* token claims fixed full name, removed apikey uniqueid equals to clientid
* **tokenexchangeauthorizationservice:** access_token is now validated using the userinfo endpoint
* **tokenservice:** removed tenantid (now tenant may be in the issuer)

<a name="1.4.3"></a>
## 1.4.3 (2024-09-19)

### Bug Fixes

* **tokenexchangeauthorizationservice:** access_token is now validated using the userinfo endpoint

<a name="1.4.2"></a>
## 1.4.2 (2024-09-18)

### Bug Fixes

* **tokenservice:** removed tenantid (now tenant may be in the issuer)

<a name="1.4.1"></a>
## 1.4.1 (2024-09-17)

### Bug Fixes

* **apikey:** removed secret from json serialize

<a name="1.4.0"></a>
## 1.4.0 (2024-09-17)

### Features

* added token exchange flow and renamed clients to api-keys

<a name="1.3.6"></a>
## 1.3.6 (2024-09-12)

### Bug Fixes

* upgrade packages

<a name="1.3.5"></a>
## 1.3.5 (2024-09-11)

### Bug Fixes

* upgrade core packages

<a name="1.3.4"></a>
## 1.3.4 (2024-09-10)

<a name="1.3.3"></a>
## 1.3.3 (2024-09-10)

### Bug Fixes

* changed all members to virtual

<a name="1.3.2"></a>
## 1.3.2 (2024-09-09)

### Bug Fixes

* **client:** removed required members

<a name="1.3.1"></a>
## 1.3.1 (2024-09-09)

### Bug Fixes

* **authenticationmiddleware:** revert from basic to bearer only in the middleware, added tests

<a name="1.3.0"></a>
## 1.3.0 (2024-09-09)

### Features

* added observable trait to entities
* removed changed property boilerplate and created a proxy for that

### Bug Fixes

* changed /token authorization to Basic

<a name="1.2.3"></a>
## 1.2.3 (2024-09-02)

### Bug Fixes

* added a base observabletype class with change tracker for props and collections

<a name="1.2.2"></a>
## 1.2.2 (2024-08-22)

### Bug Fixes

* **authorizationservice:** state and result should have serialized values

<a name="1.2.1"></a>
## 1.2.1 (2024-08-22)

### Bug Fixes

* **authorizationservice:** should not use serialized res but rather the unserialized value in roles

<a name="1.2.0"></a>
## 1.2.0 (2024-08-21)

### Features

* added parent id for not readonly childs of entities
* added property and collection modified events in entities
* **scimv2:** added patch to scimv2 routes

<a name="1.1.0"></a>
## 1.1.0 (2024-08-01)

### Features

* **scimv2 routes:** added patch endpoint map for scim v2 resources

### Bug Fixes

* change from custom enum converter to newtonsoft`s stringenumconverter

<a name="1.0.23"></a>
## 1.0.23 (2024-07-20)

### Bug Fixes

* upgrade core packages

<a name="1.0.22"></a>
## 1.0.22 (2024-07-20)

### Bug Fixes

* **routes:** fixed route naming for users, groups and clients

<a name="1.0.21"></a>
## 1.0.21 (2024-07-20)

### Bug Fixes

* upgrade core packages to 1.0.15

<a name="1.0.20"></a>
## 1.0.20 (2024-07-20)

### Bug Fixes

* added cancellation token to services

<a name="1.0.19"></a>
## 1.0.19 (2024-07-20)

### Bug Fixes

* **tokenmiddleware:** fixed authorization to string on context

<a name="1.0.18"></a>
## 1.0.18 (2024-07-19)

### Bug Fixes

* **group:** make group converter public

<a name="1.0.17"></a>
## 1.0.17 (2024-07-19)

### Bug Fixes

* entities json converters are public now

<a name="1.0.16"></a>
## 1.0.16 (2024-07-19)

### Bug Fixes

* fix packages configuration

<a name="1.0.15"></a>
## 1.0.15 (2024-07-19)

### Bug Fixes

* more clear separation of layers for middlewares
* removed open api
* upgrade core to 1.0.9

<a name="1.0.14"></a>
## 1.0.14 (2024-07-18)

### Bug Fixes

* upgraded core packages to 1.0.8

<a name="1.0.13"></a>
## 1.0.13 (2024-07-18)

### Bug Fixes

* upgraded core packages to 1.0.7

<a name="1.0.12"></a>
## 1.0.12 (2024-07-18)

### Bug Fixes

* added json schema validation to entities
* added user validation
* wIP errors and validation for models

<a name="1.0.11"></a>
## [1.0.11](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.11) (2024-07-11)

### Bug Fixes

* upgrade core pkg to 1.0.6 ([9d753ba](https://www.github.com/looplex-osi/middlewares-dotnet/commit/9d753bab55e9188780bffdb8b77c8dcbd070537b))

<a name="1.0.10"></a>
## [1.0.10](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.10) (2024-07-11)

<a name="1.0.9"></a>
## [1.0.9](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.9) (2024-07-11)

### Bug Fixes

* **scimv2routes:** added a separeted list of services per action ([20675fa](https://www.github.com/looplex-osi/middlewares-dotnet/commit/20675faa0417387f20bd3a2c3d954dcfc21181da))

<a name="1.0.8"></a>
## [1.0.8](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.8) (2024-07-11)

<a name="1.0.7"></a>
## [1.0.7](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.7) (2024-07-11)

### Bug Fixes

* upgrade core packages to 1.0.4 ([8ac657b](https://www.github.com/looplex-osi/middlewares-dotnet/commit/8ac657bd00fb290ad0e89d308fe472ea68861312))

<a name="1.0.6"></a>
## [1.0.6](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.6) (2024-07-11)

### Bug Fixes

* improved plugin service subscription ([d9d4d86](https://www.github.com/looplex-osi/middlewares-dotnet/commit/d9d4d8612f07c2b30ac9c88287a6a41a380b9c6f))

<a name="1.0.5"></a>
## [1.0.5](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.5) (2024-07-08)

### Bug Fixes

* **clientroutesextensionmethods:** fixed name of use method ([70fc2ed](https://www.github.com/looplex-osi/middlewares-dotnet/commit/70fc2ed51f26df8d9c7120eb2f090ad976246d1f))

<a name="1.0.4"></a>
## [1.0.4](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.4) (2024-07-08)

### Bug Fixes

* fix build error ([ef0031c](https://www.github.com/looplex-osi/middlewares-dotnet/commit/ef0031c238779a6c46967b2020be896b4feb852a))

<a name="1.0.3"></a>
## [1.0.3](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.3) (2024-07-08)

### Bug Fixes

* upgraded core packages to 1.0.2 ([2e8d64d](https://www.github.com/looplex-osi/middlewares-dotnet/commit/2e8d64d402b61a20b4f44d6df0b0aaba8d077e66))

<a name="1.0.2"></a>
## [1.0.2](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.2) (2024-07-02)

### Bug Fixes

* added SCIM v2 middleware ([7119b34](https://www.github.com/looplex-osi/middlewares-dotnet/commit/7119b3472ebb29586ace01074d19661335adabd3))

<a name="1.0.1"></a>
## [1.0.1](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.1) (2024-07-02)

<a name="1.0.0"></a>
## [1.0.0](https://www.github.com/looplex-osi/middlewares-dotnet/releases/tag/v1.0.0) (2024-07-01)

