namespace AllTodos

open FSharpPlus
open CQRS
open DomainError
open Todo
open Responder.JsonResponder

module AllTodos =
  type request = Unit
  type DataStore = DataSource<Todo.t, DomainError>
  let query (source: DataStore) _ = source.All
  let path = "/todos"

  let responder (dataStore: DataStore) context =
    dataStore.All
    |> Async.map (
      Result.either (fun all -> ok all context) (fun e -> withError e context)
    )
    |> Async.RunSynchronously
