namespace ATodo

open Falco
open Todo
open CQRS
open DomainError
open FSharpPlus
open System

module ATodo =
  type DataStore = DataSource<Todo.t, DomainError>
  let path = "/todo/{id}"

  let responder (dataSource: DataStore) context =
    let id = (Request.getRoute context).GetString "id" |> Guid.Parse

    dataSource.Find(fun (item: Todo.t) -> item.Id.Equals(id))
    |> Async.map (
      Result.either (fun x -> Response.ofJson x context) (fun _ ->
        Response.ofJson null context)
    )
    |> Async.RunSynchronously
