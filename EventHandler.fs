module EventHandler

open FSharpPlus
open CQRS
open Todo
open DomainError

let eventHandler (objectSource: DataSource<Todo.t, DomainError>) =
    fun (inbox: MailboxProcessor<Dispatch.Reply<Event, Todo.t, DomainError>>) ->
        let rec messageLoop () =
            async {
                let! (msg: Dispatch.Reply<Event, Todo.t, DomainError>) = inbox.Receive()

                match msg with
                | Dispatch.NoReply _ -> ()
                | Dispatch.Reply(TodoCreated(id, title), channel) ->

                Todo.create id title
                |> objectSource.Store
                |> Async.RunSynchronously
                |> channel.Reply

                return! messageLoop ()
              }

        messageLoop ()
