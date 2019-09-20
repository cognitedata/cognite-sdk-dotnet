// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Login

open Oryx

type LoginStatusEntity internal (user: string, loggedIn: bool, project: string, projectId: int64, apiKeyId: int64) =

    member val User : string = user with get, set
    member val LoggedIn : bool = loggedIn with get, set
    member val Project : string = project with get, set
    member val ProjectId : int64 = projectId with get, set
    member val ApiKeyId : int64 = apiKeyId with get, set

    // Create new Event.
    new () =
        LoginStatusEntity(user=null, loggedIn=false, project=null, projectId=0L, apiKeyId=0L)

type LoginStatusDto = {
    User: string
    LoggedIn: bool
    Project: string
    ProjectId: int64
    ApiKeyId: int64 option
} with
    /// Translates the domain type to a plain old crl object
    member this.ToLoginStatusEntity () : LoginStatusEntity =
        let apiKeyId = if this.ApiKeyId.IsSome then this.ApiKeyId.Value else 0L

        LoginStatusEntity(
            User = this.User,
            LoggedIn = this.LoggedIn,
            Project = this.Project,
            ProjectId = this.ProjectId,
            ApiKeyId = apiKeyId
        )



type LoginStatusItemsDto = {
    Data: LoginStatusDto
}

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

