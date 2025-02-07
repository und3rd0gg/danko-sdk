var functions = {

  SDK_Init: function(devID,pubID) {

    devID = UTF8ToString(devID);
    pubID = UTF8ToString(pubID);

    (function(d, s, id) {
      var js,
        fjs = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) return;
      js = d.createElement(s);
      js.id = id;

      js.onload=function(){
        LaggedAPI.init(devID, pubID);
      }

      js.src = "//lagged.com/api/rev-share/lagged.js";

      fjs.parentNode.insertBefore(js, fjs);
    })(document, "script", "lagged-jssdk");
  },

  SDK_CallHighScore: function(score, board_id) {

    board_id=UTF8ToString(board_id);

    var boardinfo={};
    boardinfo.score=score;
    boardinfo.board=board_id;
    LaggedAPI.Scores.save(boardinfo, function(response) {
    if(response.success) {
    console.log('high score saved')
    }else {
    console.log(response.errormsg);
    }
    });

  },

  SDK_SaveAchievement: function(award) {

    award=UTF8ToString(award);

    var api_awards=[];
    api_awards.push(award);
    LaggedAPI.Achievements.save(api_awards, function(response) {
    if(response.success) {
    console.log('achievement saved')
    }else {
    console.log(response.errormsg);
    }
    });

  },

  SDK_ShowAd: function() {

    SendMessage("LaggedAPIUnity", "PauseGameCallback");

    LaggedAPI.APIAds.show(function() {

        SendMessage("LaggedAPIUnity", "ResumeGameCallback");

    });

  },

  SDK_CheckRewardAd: function(){

    this.rewardAdAvailable=false;
    this.rewardAdFunction=function(){};

    LaggedAPI.GEvents.reward(function(success, showAdFn){
      if(success){
        SendMessage("LaggedAPIUnity", "RewardAdReadyCallback");
        this.rewardAdAvailable=true;
        this.rewardAdFunction=showAdFn;
      }
    }, function(success){
      if(success){

        SendMessage("LaggedAPIUnity", "RewardAdSuccessCallback");

      }else{

        SendMessage("LaggedAPIUnity", "RewardAdFailCallback");

      }
      this.rewardAdAvailable=false;
      this.rewardAdFunction=function(){};
    });

  },

  SDK_PlayRewardAd: function(){
    if(this.rewardAdAvailable){

      this.rewardAdFunction();

    }
  },

};

mergeInto(LibraryManager.library, functions);
