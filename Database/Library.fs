namespace Database

open FSharpPlus
open DomainError

type DataSource<'a, 'e> =
  abstract member All: Async<Result<'a list, 'e>>
  abstract member Count: Async<Result<int, 'e>>
  abstract member Store: 'a -> Async<Result<'a, 'e>>
  abstract member Find: ('a -> bool) -> Async<Result<'a, 'e>>

type InMemory<'a>() =
  let mutable store: 'a list = []

  interface DataSource<'a, DomainError> with
    member this.All = async { return Result.Ok(store) }
    member this.Count = async { return Ok(store.Length) }

    member this.Store item =
      async {
        do store <- List.append [ item ] store
        return Ok(item)
      }

    member this.Find f =
      async {
        return
          match List.tryFind f store with
          | Some(item) -> Ok(item)
          | _ -> Error(NotFound)
      }
