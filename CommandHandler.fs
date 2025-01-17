module CommandHandler

open FSharpPlus
open CQRS
open Database
open Todo
open DomainError
open Command
open System

type EventDispatch = Dispatch.Reply<Command, Todo.t, DomainError>

let commandHandler
  (eventSource: DataSource<Event.Event, DomainError>)
  (eventHandler:
    MailboxProcessor<Dispatch.Reply<Event.Event, Todo.t, DomainError>>)
  =
  fun (inbox: MailboxProcessor<EventDispatch>) ->
    let rec messageLoop () =
      async {
        let! (msg: Dispatch.Reply<Command, Todo.t, DomainError>) =
          inbox.Receive()

        match msg with
        | Dispatch.NoReply cmd -> ()
        | Dispatch.Reply(cmd, replyChannel) ->
          match cmd with
          | CreateTodo title ->
            let event = Event.TodoCreated(Guid.NewGuid(), title)
            eventSource.Store event |> Async.RunSynchronously |> ignore // TODO(dias): handle error

            eventHandler.PostAndAsyncReply(fun replyChannel ->
              Dispatch.Reply(event, replyChannel))
            |> Async.RunSynchronously
            |> replyChannel.Reply

            return! messageLoop ()
      }

    messageLoop ()
