using System.Collections;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System;

namespace Tests
{
    public class TestSuite
    {
        // A Test behaves as an ordinary method
        /*        [Test]
                public void TestSuiteSimplePasses()
                {
                    // Use the Assert class to test conditions
                }*/

        [SetUp]
        public void SetupTests()
        {
            PlayerPrefs.DeleteAll();
        }

        [TearDown]
        public void TeardownTests()
        {
            PlayerPrefs.DeleteAll();
        }

        [UnityTest]
        public IEnumerator LoadMainMenu()
        {
            // load main menu
            var obj = new MonoBehaviourTest<TestDesktopScene>(true);
            yield return obj;
            Assert.IsTrue(obj.gameObject.GetComponent<TestDesktopScene>().TestPass);
        }
    }

    // assumes in setup that player preferences have been erased
    public class TestDesktopScene : MonoBehaviour, IMonoBehaviourTest
    {
        private const string COMPANY_AND_CHAR_NAME = "anything i guess";
        public bool TestPass { get; private set; } = false;
        public bool IsTestFinished { get; private set; } = false;

        public void Awake()
        {
            StartCoroutine(TestSuiteWithEnumeratorPasses());
        }

        public IEnumerator TestSuiteWithEnumeratorPasses()
        {
            var oneframe = new WaitForEndOfFrame();
           // var keyboard = InputSystem.AddDevice<Keyboard>();
           // var mouse = InputSystem.AddDevice<Mouse>();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            // wait until scene is the active scene (aka it's loaded, since it's single load mode)
            while (SceneManager.GetActiveScene().name != "MainMenu")
                yield return null;
            var mainMenu = CheckMainMenu();
            yield return oneframe;
            CheckLoadMenu();
            yield return oneframe;
            mainMenu.Play();
            while (SceneManager.GetActiveScene().name != "Desktop")
                yield return null;
            yield return oneframe;
            var charCustom = CheckNewGame();
            yield return oneframe;
            charCustom.confirm();
            yield return oneframe;
            CustomizationWasSaved();
            SaveGameAndQuit();
            TestPass = true;
            IsTestFinished = true;
        }

        private static void CustomizationWasSaved()
        {
            var playerdata = PlayerData.Player;
            Assert.IsTrue(playerdata.Name == COMPANY_AND_CHAR_NAME);
            Assert.IsTrue(playerdata.CompanyName == COMPANY_AND_CHAR_NAME);
        }

        private static void SaveGameAndQuit()
        {
            
        }

        private static MainMenu CheckMainMenu()
        {
            var mainMenu = FindObjectOfType<MainMenu>();
            // menu must exist in main menu scene
            Assert.IsNotNull(mainMenu);
            // cannot click continue button with no saves registered
            Assert.IsFalse(mainMenu.continueB.isActiveAndEnabled);
            mainMenu.Load();
            return mainMenu;
        }

        private static CharacterContentManager CheckNewGame()
        {
            // desktop should have loaded by now
            // and one frame has passed
            var charCustom = FindObjectOfType<CharacterContentManager>();
            Assert.IsNotNull(charCustom);
            Assert.IsTrue(charCustom.isActiveAndEnabled);
            var nameandCompanyInputs = charCustom.GetComponentsInChildren<TMPro.TMP_InputField>().ToList();
            // Debug.LogWarning(string.Join(", ",nameandCompanyInputs.Select(x => x.name)));
            Assert.IsTrue(nameandCompanyInputs.Count == 2);

            nameandCompanyInputs[0].text = COMPANY_AND_CHAR_NAME;
            nameandCompanyInputs[1].text = COMPANY_AND_CHAR_NAME;
            return charCustom;
        }

        private static void CheckLoadMenu()
        {
            var loadMenu = FindObjectOfType<LoadMenu>();
            Assert.IsTrue(loadMenu.gameObject.activeSelf);
            var loadButtons = new List<Button>();
            loadButtons.AddRange(loadMenu.gameObject.GetComponentsInChildren<Button>());
            // only the return button should be active when theres no save games
            // Debug.LogWarning(loadButtons.Aggregate("", (acc, x) => acc + x.gameObject.name));
            Assert.IsTrue(loadButtons.Aggregate(0,
               (acc, x) => x.isActiveAndEnabled ? acc + 1 : acc) == 1);
            loadMenu.Return();
        }
    }

}
