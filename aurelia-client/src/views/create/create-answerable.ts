import { HttpClient, IRouter, IRouteViewModel, Params } from "aurelia";
import { EType } from "../../domain/enums/EType";
import { IAnswerable, IAnswerableCreate } from "../../domain/IAnswerable";
import { AnswerableService } from "../../services/answerable-service";
import { AppState } from "../../state/app-state";


export class CreateAnswerable implements IRouteViewModel {
    private model: IAnswerableCreate = 
    {
        name: '',
        description: '',
        type: null
    }
    private errorMessage: string = '';
    private returnUrl: string;

    constructor(@IRouter private router: IRouter, private httpClient: HttpClient, private appState: AppState) {
    }

    async load(params: Params) {
        this.returnUrl = params[0];
    }

    async onSubmit() {
        this.errorMessage = '';
        if (!this.validate()) return

        const service = new AnswerableService(this.httpClient, this.appState)

        const response = await service.create({...this.model, type: this.model.type.toString() === EType.Quiz.toString() ? 0 : 1});

        if (response.statusCode === 201) {
            this.router.load(`/answerable-router/edit-answerable(${response.data.id})`)
        } else {
            this.errorMessage = response.errorMessage
        }
    }

    validate() {
        if (!this.model.name) {
            this.errorMessage = "Name is required."
            return false
        }
        if (!this.model.description) {
            this.errorMessage = "Description is required."
            return false
        }
        if (!this.model.type) {
            this.errorMessage = "Type is required."
            return false
        }
        return true
    }

    backToList() {
        this.router.load("/answerable-router/" + this.returnUrl)
    }
}