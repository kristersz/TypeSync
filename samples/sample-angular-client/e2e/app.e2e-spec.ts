import { SampleAngularClientPage } from './app.po';

describe('sample-angular-client App', () => {
  let page: SampleAngularClientPage;

  beforeEach(() => {
    page = new SampleAngularClientPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
