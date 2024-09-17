# Identity and Resources Management

SCIM (System for Cross-domain Identity Management) is considered an industry standard for identity management and user provisioning across multiple applications and domains. It has been widely adopted by cloud service providers, SaaS applications, and enterprise solutions due to its interoperability, scalability, and ease of integration.

Many major tech companies, including Microsoft, Amazon, Google and others, have implemented SCIM in their identity and access management services. The protocol can be resumed by the following er-diagram, operations and discovery mechanism:

```mermaid
erDiagram
  RESOURCE {
    uuid id
    string externalId
  }

  META {
    string resourceType
    iso8601 created
    iso8601 lastModified
    string location
    string version
  }

  USER {
    string userName
    string displayName
    string nickName
    rfc3986 profileUrl
    string title
    string userType
    rfc7231 preferredLanguage
    rfc5646 locale
    rfc6557 timezone
    boolean active
    deprecated password
  }

  GROUP {
    string displayName
  }

  OTHER {}

  NAME {
    string formatted
    string familyName
    string givenName
    string middleName
    string honorificPrefix
    string honorificSuffix
  }

  EMAIL {
    rfc5321 display
    string type
  }

  PHONE {
    rfc3966 display
    string type
  }

  IMS {
    string display
    string type
  }

  PHOTOS {
    rfc3986 uri
    string type
  }

  ADDRESS {
    string formatted
    string streetAddress
    string locality
    string region
    string postalCode
    iso3166-1 country
  }

  ENTITLEMENTS {
    string entitlement
  }

  ROLES {
    string role
  }

  X509CERTIFICATES {
    base64 certificateDer
  }

  ENTERPRISE_USER {
    string employeeNumber
    string costCenter
    string organization
    string division
    string department
  }

  MANAGER {
    uuid value
    rfc3986 dollar_ref
    string displayName
  }

  RESOURCE ||--|| META : contains
  USER ||--|| RESOURCE : "is a"
  GROUP ||--|| RESOURCE : "is a"
  OTHER ||--|| RESOURCE : "is a"

  ENTERPRISE_USER ||--|| USER : "is a"
  ENTERPRISE_USER ||--|| MANAGER : "has a"

  USER ||--o| NAME : has
  USER ||--o{ EMAIL : has
  USER ||--o{ PHONE : has
  USER ||--o{ IMS : has
  USER ||--o{ PHOTOS : has
  USER ||--o{ ADDRESS : has
  USER ||--o{ ENTITLEMENTS : has
  USER ||--o{ ROLES : has
  USER ||--o{ X509CERTIFICATES : has

  USER ||--o{ GROUP : "belongs to"
```

**Operations**

| action      | verb     | url                                                                                                                         |
|-------------|----------|-----------------------------------------------------------------------------------------------------------------------------|
| **Create**  | `POST`   | https://domain.com/:version/:resource                                                                                       |
| **Read**    | `GET`    | https://domain.com/:version/:resource/:id                                                                                   |
| **Replace** | `PUT`    | https://domain.com/:version/:resource/:id                                                                                   |
| **Delete**  | `DELETE` | https://domain.com/:version/:resource/:id                                                                                   |
| **Update**  | `PATCH`  | https://domain.com/:version/:resource/:id                                                                                   |
| **Search**  | `GET`    | https://domain.com/:version/:resource?ﬁlter={attribute}{op}{value}&sortBy={attributeName}&sortOrder={ascending\|descending} |
| **Bulk**    | `POST`   | https://domain.com/:version/Bulk                                                                                            |

**Discovery**

To simplify interoperability, SCIM provides three end points to discover supported features and specific attribute details:

| discover                                                      | verb  | url                                               |
|---------------------------------------------------------------|-------|---------------------------------------------------|
| Specification compliance, authentication schemes, data models | `GET` | https://domain.com/:version/ServiceProviderConfig |
| Types of resources available                                  | `GET` | https://domain.com/:version/ResourceTypes         |
| Resources and attribute extensions                            | `GET` | https://domain.com/:version/Schemas               |

## Extensibility

Additionally to being globally recognized and used as protocol for sharing and synchronizing Users and Groups, the specification also defines a standardized way to extend it for other resource types, providing a robust foundation for exchanging serialized data across multiple domains using REST architectural style. Because of that, we feel there's only one way to define RESTful APIs, any other attempt would introduce unnecessary complexity, duplicating what SCIM already does efficiently. This not only saves development time and resources but also ensures compatibility with existing systems and services, making integration smoother and more predictable.

## References

* [REST](https://ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm) -- Representational State Transfer
* [RFC3966](https://datatracker.ietf.org/doc/html/rfc3966) -- The tel URI for Telephone Numbers
* [RFC3986](https://datatracker.ietf.org/doc/html/rfc3986) -- Uniform Resource Identifier (URI): Generic Syntax
* [RFC4648](https://datatracker.ietf.org/doc/html/rfc4648) -- The Base16, Base32, and Base64 Data Encodings
* [RFC5321](https://datatracker.ietf.org/doc/html/rfc5321) -- Simple Mail Transfer Protocol
* [RFC5646](https://datatracker.ietf.org/doc/html/rfc5646) -- Tags for Identifying Languages
* [RFC6557](https://datatracker.ietf.org/doc/html/rfc6557) -- Procedures for Maintaining the Time Zone Database
* [RFC7642](https://datatracker.ietf.org/doc/html/rfc7642) -- System for Cross-domain Identity Management: Definitions, Overview, Concepts, and Requirements
* [RFC7643](https://datatracker.ietf.org/doc/html/rfc7643) -- System for Cross-domain Identity Management: Core Schema
* [RFC7644](https://datatracker.ietf.org/doc/html/rfc7644) -- System for Cross-domain Identity Management: Protocol
* [ISO639](https://www.iso.org/iso-639-language-code) -- Language code
* [ISO3166](https://www.iso.org/iso-3166-country-codes.html) -- Country Codes

# Authentication and Authorization

When examining the public or confidential nature of a given client, we're evaluating the ability of that client to prove its identity to the authorization server. This is important because the authorization server must be able to trust the identity of the client in order to issue access tokens.

* **Public client applications** run on devices, such as desktop, browserless APIs, mobile or client-side browser apps. They can't be trusted to safely keep application secrets, so they can only access web APIs on behalf of the user. Anytime the source or compiled bytecode of a given app is transmitted anywhere it can be read, disassembled, or otherwise inspected by untrusted parties, it's a public client. As they also only support public client flows and can't hold configuration-time secrets, they can't have client secrets.
* **Confidential client applications** run on servers, such as web apps, web API apps, or service/daemon apps. They're considered difficult to access by users or attackers, and therefore can adequately hold configuration-time secrets to assert proof of its identity. The client ID is exposed through the web browser, but the secret is passed only in the back channel and never directly exposed.

```mermaid
graph LR
  actor([🧑])
  pca[[Public Client Application - PCA]]
  cca[[Confidential Client Application - CCA]]
  
  actor --> pca
  actor --> cca
```

> IMPORTANT: Both app types SHOULD maintain a user token cache and can acquire a token silently (when the token is present in the cache).

## PCA (Graphical User Interface Flow)

We follow the [Open ID Connect (OIDC) Flow](https://openid.net/specs/openid-connect-core-1_0-final.html) for authentication and [OAuth 2.0 Token Exchange](https://datatracker.ietf.org/doc/html/rfc8693) for authorization:

```mermaid
sequenceDiagram
  autonumber
  actor U as Resource Owner
  box rgba(108, 0, 0, .1) Client
    participant App as Web App<br>e.g. saas.looplex.com
  end
  box rgba(0, 108, 0, .1) Authentication Server
    participant AuthN as OpenID Provider<br>e.g. Microsoft Entra
  end
  box rgba(0, 0, 108, .1) Resource Server
    participant API as Microservice<br>e.g. cases.looplex.com/api
  end

  loop
    U ->> App: Interacts with
    activate App
    App ->> App: Is user authenticated?
    alt not authenticated
      App -->> U: OIDC front door url (+client_id)
      deactivate App
      U -->> AuthN: is redirected to
      activate AuthN
      AuthN ->> U: Ask for credentials
      U -->> AuthN: credentials
      AuthN ->> U: Ask for consent
      U -->> AuthN: consent
      AuthN ->> U: redirect url (+authorization_code)
      U ->> App: is redirected to
      activate App
      App ->> AuthN: authorization_code
      AuthN -->> App: id_token, access_token[, refresh_token]
      deactivate AuthN
      App -->> API: POST /token<br>Content-Type: application/x-www-form-urlencoded
      activate API
      Note over App,API: grant_type=urn:ietf:params:oauth:grant-type:token-exchange<br>subject_token={{access_token}}<br>subject_token_type=urn:ietf:params:oauth:token-type:access_token
      API ->> AuthN: GET /userinfo<br>Authorization: Bearer {{access_token}}
      activate AuthN
      AuthN -->> API: name, email, phone, photo
      deactivate AuthN
      API ->> API: Has active subscription?
      alt active module subscription
        API ->> API: generate access_token (2)
        API -->> App: access_token[, refresh_token] (2)
        App -->> U: Render Landing Page
      else invalid/no module subscription
        API -->> App: INVALID_REQUEST
        App -->> U: Render Product Led Growth Page
      end
      App ->> API: Request<br>Authorization: Bearer {{access_token (2)}}
      API -->> App: Response
    deactivate API
      App -->> U: Render Protected UI
    end
  end
  deactivate App
```

## CCA (Application Programming Interface Flow)

In this pattern, a pair of values is generated by the authorization server when registering a client. The client ID is a public value that identifies the application, while the client secret is a confidential value used to prove the identity of the application. We follow the [OAuth 2.0 Client Credentials Flow](https://datatracker.ietf.org/doc/html/rfc6749#section-4.4) which is typically used for server-to-server communication and automated scripts requiring no user interaction. TL;DR; Initially:

```mermaid
sequenceDiagram
  autonumber
  actor U as Resource Owner
  box rgba(108, 0, 0, .1) Client
    participant App as Web App<br>e.g. actions.looplex.com
  end
  box rgba(0, 0, 108, .1) Resource Server
    participant API as Microservice<br>e.g. actions.looplex.com/api
  end
  box rgba(0, 108, 0, .1) 3rd Party
    participant Svc as Service<br>e.g. Postman
  end

  U ->> App: I want to integrate to another app
  activate App
  App -->> U: Render API Integration form
  U ->> App: Other app id, name, expiration, permissions etc
  App ->> API: POST /api-keys<br>Authorization: Bearer {{access_token}}
  activate API
  API -->> App: client_id, client_secret
  deactivate API
  App -->> U: client_id, client_secret
  deactivate App
  U --) Svc: client_id, client_secret
```

And later:

```mermaid
sequenceDiagram
  autonumber
  actor U as Resource Owner
  box rgba(0, 108, 0, .1) 3rd Party
    participant Svc as Service<br>e.g. Postman
  end
  box rgba(0, 0, 108, .1) Resource Server
    participant API as Microservice<br>e.g. actions.looplex.com/api
  end

  U ->> Svc: Create or Execute something
  activate Svc
  Svc ->> API: POST /token<br>Content-Type: application/x-www-form-urlencoded<br>Authorization: Basic {{base64FromString(client_id:client_secret)}}
  activate API
  Note over Svc,API: grant_type=client_credentials
  API -->> Svc: access_token[, refresh_token]
  loop
  Svc ->> API: Request<br>Authorization: Bearer {{access_token}}
  API -->> Svc: Response
  end
  deactivate API
  Svc -->> U: Result
  deactivate Svc
```

## `access_token` payload schema

```json
{
  "type": "object",
  "properties": {
    "name": { "type": "string", "description": "User's full name, including all middle names, titles, and suffixes as appropriate, formatted for display (e.g., 'Ms. Barbara Jane Jensen, III')." },
    "email": { "type": "string", "format": "email", "description": "User's internet email address, see RFC 5321, section 4.1.2." },
    "phone": { "type": "string", "format": "tel", "description": "User's telephone number as E.164." },
    "photo": { "type": "string", "format": "uri", "description": "User's avatar as a universal resource identifier (URI), according to RFC3986." },
    "preferredLanguage": { "type": "string", "description": "Indicates the user's preferred written or spoken languages and is generally used for selecting a localized user interface." }
  },
  "required": ["name", "email"]
}
```

> IMPORTANT: Although `preferredLanguage` SHOULD be present in the `access_token` payload, it have a lower specificity than a local_storage `currentLanguage` value.

## ER-Diagram

```mermaid
erDiagram
  Users {
    int id PK
    uuid uid
    varchar name
  }

  ApiKeys {
    int id PK
    int user_id FK
    varchar client_name
    uuid client_id
    varchar digest
  }

  Users ||--o{ ApiKeys : contains
```
## PBKDF Reference Implementation

```js
import { scrypt, randomBytes, timingSafeEqual } from 'node:crypto'

const DEFAULT_SALT_LENGTH = 16// NIST 800-132 minimal recommended salt length
const DEFAULT_KEY_LENGTH = 32

async function pbkdf (password, options = {}) {
  return new Promise((resolve, reject) => {
    let saltlen = options.saltlen ?? DEFAULT_SALT_LENGTH
    let keylen = options.keylen ?? DEFAULT_KEY_LENGTH
    let salt = randomBytes(saltlen)
    scrypt(password, salt, keylen, options, (err, derivedKey) => {
      if (err) reject(err)
      resolve(`${salt.toString('hex')}:${derivedKey.toString('hex')}`)
    })
  })
}

async function pbkdf_verify (password, digest, options = {}) {
  return new Promise((resolve, reject) => {
    let [salt, providedDerivedKey] = digest.split(':').map(x => Buffer.from(x, 'hex'))
    let keylen = options.keylen ?? DEFAULT_KEY_LENGTH
    scrypt(password, salt, keylen, options, (err, derivedKey) => {
      if (err) reject(err)
      resolve(timingSafeEqual(providedDerivedKey, derivedKey))
    })
  })
}

let secret_1 = await pbkdf('very_secret_password')
let secret_2 = await pbkdf('very_secret_password')

console.log(secret_1.length)

// digest MUST be different
console.log(secret_1)
console.log(secret_2)
console.log(secret_1 == secret_2)

// incoming password MUST be verifiable
console.log(await pbkdf_verify('very_secret_password', secret_1))
console.log(await pbkdf_verify('very_secret_password', secret_2))
```

## References

* [RFC2119](https://www.ietf.org/rfc/rfc2119.txt) -- Key words for use in RFCs to Indicate Requirement Levels
* [RFC6749](https://www.ietf.org/rfc/rfc6749.txt) -- The OAuth 2.0 Authorization Framework
* [RFC6750](https://www.ietf.org/rfc/rfc6750.txt) -- The OAuth 2.0 Authorization Framework: Bearer Token Usage
* [RFC7519](https://www.ietf.org/rfc/rfc7519.txt) -- JSON Web Token (JWT)
* [RFC7521](https://www.ietf.org/rfc/rfc7521.txt) -- Assertion Framework for OAuth 2.0 Client Authentication and Authorization Grants
* [RFC7523](https://www.ietf.org/rfc/rfc7523.txt) -- JSON Web Token (JWT) Profile for OAuth 2.0 Client Authentication and Authorization Grants
* [RFC8693](https://www.ietf.org/rfc/rfc8693.txt) -- OAuth 2.0 Token Exchange
* [OIDC](https://openid.net/specs/openid-connect-core-1_0-final.html) -- OpenID Connect Core 1.0 Final
* [Scrypt | Crypto | Node.js](https://nodejs.org/api/crypto.html#cryptoscryptpassword-salt-keylen-options-callback) -- crypto.scrypt(password, salt, keylen[, options], callback)
* [NIST Special Publication 800-132](https://nvlpubs.nist.gov/nistpubs/Legacy/SP/nistspecialpublication800-132.pdf) -- Recommendation for Password-Based Key Derivation
