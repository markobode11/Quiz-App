import { IRouter } from "aurelia";
import { AppState } from "./state/app-state";

export class MyApp {

  constructor(
    @IRouter private router: IRouter,
    private state: AppState) {

  }
  
  async logOut(){
    this.state.token = null;
    this.state.firstname = '';
    this.state.lastname = '';
    this.state.isAdmin = false;
    document.body.style.backgroundColor = "#587c94"

    await this.router.load('/home-index');
  }

}
