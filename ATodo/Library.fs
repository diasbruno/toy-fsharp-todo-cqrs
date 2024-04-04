namespace ATodo

open Falco
open Todo
open Database
open DomainError
open FSharpPlus
open System
open Responder.JsonResponder

module ATodo =
  type DataStore = DataSource<Todo.t, DomainError>
  let path = "/todo/{id}"

  let responder (dataSource: DataStore) context =
    let id = (Request.getRoute context).GetString "id" |> Guid.Parse

    dataSource.Find(fun (item: Todo.t) -> item.Id.Equals(id))
    |> Async.map (
      Result.either (fun x -> ok x context) (fun _ -> withError Unknown context)
    )
    |> Async.RunSynchronously
