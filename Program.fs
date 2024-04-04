module making_projects_simplier_cqrs.Program

open Falco.Routing
open Falco.HostBuilder
open ATodo
open AllTodos
open CreateTodo
open CQRS
open Todo
open Database
open CommandHandler
open EventHandler

[<EntryPoint>]
let main args =
  let eventSource = (new InMemory<Event.Event>())
  let objectSource = (new InMemory<Todo.t>())
  let eventHandler = MailboxProcessor.StartImmediate(eventHandler objectSource)

  let commandHandler =
    MailboxProcessor.StartImmediate(commandHandler eventSource eventHandler)

  webHost args {
    endpoints
      [ get AllTodos.path (AllTodos.responder objectSource)
        post CreateTodo.path (CreateTodo.responder commandHandler)
        get ATodo.path (ATodo.responder objectSource) ]
  }

  0
