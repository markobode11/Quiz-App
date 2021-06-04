import { HttpClient } from "aurelia";
import { IRouter } from "aurelia";
import { AccountService } from "../../services/account-service";
import { AppState } from "../../state/app-state";
import { IJwt } from "../../types/IJwt";
import jwt_decode from "jwt-decode";

export class IdentityLogin {

    private service: AccountService =
        new AccountService(this.httpClient);

    private email!: string;
    private password!: string;
    private error!: string;

    constructor(
        @IRouter private router: IRouter,
        private state: AppState,
        protected httpClient: HttpClient) {

    }

    async loginClicked(event: Event) {
        event.preventDefault();
        event.stopPropagation();

        const response = await this.service.login(this.email, this.password);

        if (response.statusCode == 200 && response.data) {
            this.state.token = (response.data as IJwt).token;
            this.state.firstname = (response.data as IJwt).firstname;
            this.state.lastname = (response.data as IJwt).lastname;

            var decodedToken = jwt_decode(this.state.token)
            var role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
            if (role === "Admin") {
                this.state.isAdmin = true; document.body.style.backgroundColor = '#587c77';
            }

            await this.router.load('/home-index');
        } else {
            this.error = response.errorMessage!
        }
    }
}
