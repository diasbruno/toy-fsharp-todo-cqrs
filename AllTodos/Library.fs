namespace AllTodos

open FSharpPlus
open Falco
open CQRS
open DomainError
open Todo

module AllTodos =
  type request = Unit
  type DataStore = DataSource<Todo.t, DomainError>
  let query (source: DataStore) _ = source.All
  let path = "/todos"

  let responder (dataStore: DataStore) context =
    dataStore.All
    |> Async.map (
      Result.either (fun e -> Response.ofJson e context) (fun all ->
        Response.ofJson all context)
    )
    |> Async.RunSynchronously
