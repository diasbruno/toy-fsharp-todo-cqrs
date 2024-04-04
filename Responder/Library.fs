namespace Responder

open Falco
open Microsoft.AspNetCore.Http
open DomainError

module JsonResponder =
  let withStatus status data (context: HttpContext) =
    context |> Response.withStatusCode status |> Response.ofJson data

  let ok data (context: HttpContext) = withStatus 200 data context

  let withError (error: DomainError) (context: HttpContext) =
    (match error with
     | NotFound -> withStatus 404 null
     | Unknown -> withStatus 500 null)
      context
