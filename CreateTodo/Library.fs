namespace CreateTodo

open FSharpPlus
open FSharpPlus.Data
open Falco
open CQRS
open DomainError
open Todo

module CreateTodo =
    type Input = { Title: string }

    type Handler = Handler<Todo.t, Input, Todo.t, DomainError>

    let path = "/todo"

    type ValidationError = EmptyTitle

    let validate (input: Input) =
        if input.Title = "" then
            Failure ValidationError.EmptyTitle
        else
            Success (CreateTodo input.Title)

    let run (handler: MailboxProcessor<Dispatch.Reply<Command, Todo.t, DomainError>>) (input: Input) context =
      validate input
      |> Validation.either (fun e ->
                            Response.ofJson e context)
                           (fun (command: Command) ->
                            Dispatch.sync handler command
                            |> Result.either (fun item -> Response.ofJson item context)
                                             (fun _ -> Response.ofJson null context))

    let responder handler context =
      Request.mapJson (run handler) context
