module making_projects_simplier_cqrs.Program

open System
open FSharpPlus
open Falco.Routing
open Falco.HostBuilder
open AllTodos
open CreateTodo
open CQRS
open Todo
open DomainError
open Database

type EventDispatch = Dispatch.Reply<Command, Todo.t, DomainError>

let commandHandler (eventSource: DataSource<Event, DomainError>) (eventHandler: MailboxProcessor<Dispatch.Reply<Event, Todo.t, DomainError>>) =
  fun (inbox: MailboxProcessor<EventDispatch>) ->
    let rec messageLoop () =
      async { let! (msg: Dispatch.Reply<Command, Todo.t, DomainError>) = inbox.Receive()

              match msg with
              | Dispatch.NoReply cmd -> ()
              | Dispatch.Reply(cmd, replyChannel) ->
              match cmd with
              | CreateTodo title ->
              let event = TodoCreated(Guid.NewGuid(),  title)
              eventSource.Store event
              |> Async.RunSynchronously
              |> ignore // TODO(dias): handle error

              eventHandler.PostAndAsyncReply(fun replyChannel -> Dispatch.Reply(event, replyChannel))
              |> Async.RunSynchronously
              |> replyChannel.Reply

              return! messageLoop () }

    messageLoop ()

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

[<EntryPoint>]
let main args =
    let eventSource = (new InMemory<Event>())
    let objectSource = (new InMemory<Todo.t>())
    let eventHandler = MailboxProcessor.StartImmediate(eventHandler objectSource)
    let commandHandler = MailboxProcessor.StartImmediate(commandHandler eventSource eventHandler)

    webHost args {
        endpoints
            [ get AllTodos.path (AllTodos.responder objectSource)
              post CreateTodo.path (CreateTodo.responder commandHandler) ]
    }

    0
