import { HttpClient } from "aurelia";
import { IRouter } from "aurelia";
import { AccountService } from "../../services/account-service";
import { AppState } from "../../state/app-state";
import { IJwt } from "../../types/IJwt";

export class IdentityRegister {

    private service: AccountService =
        new AccountService(this.httpClient);

    private email!: string;
    private password!: string;
    private lastname!: string;
    private firstname!: string;
    private error!: string;

    constructor(
        @IRouter private router: IRouter,
        private state: AppState,
        protected httpClient: HttpClient) {

    }

    async registerClicked(event: Event) {
        event.preventDefault();
        event.stopPropagation();

        let response = await this.service.register(this.email, this.password, this.firstname, this.lastname);

        if (response.statusCode == 200 && response.data ) {
            this.state.token = (response.data as IJwt).token;
            this.state.firstname = (response.data as IJwt).firstname;
            this.state.lastname = (response.data as IJwt).lastname;

            await this.router.load('/home-index');
        } else {
            this.error = response.errorMessage!
        }
    }
}